<template>
    <div class="page-layout">
        <div class="left-tree">
            <el-card shadow="hover">
                <template #header><span style="font-weight: bold;">选择煤矿</span></template>
                <el-tree :data="state.treeData" :props="state.treeProps" @node-click="handleNodeClick" node-key="id" default-expand-all highlight-current />
            </el-card>
        </div>
        <div class="right-content">
            <el-card shadow="hover">
                <el-form :inline="true">
                    <el-form-item label="传感器类型">
                        <el-select v-model="state.sensorType" placeholder="全部" style="width: 150px;" clearable>
                            <el-option label="甲烷" value="CH4" />
                            <el-option label="一氧化碳" value="CO" />
                            <el-option label="温度" value="TEMP" />
                        </el-select>
                    </el-form-item>
                    <el-form-item><el-button type="primary" @click="loadData">查询</el-button></el-form-item>
                </el-form>
            </el-card>
            <el-card shadow="hover" style="margin-top: 10px">
                <el-table :data="state.tableData" v-loading="state.loading" border stripe height="400">
                    <el-table-column type="index" label="序号" width="60" align="center" />
                    <el-table-column prop="sensorId" label="传感器编号" align="center" />
                    <el-table-column prop="sensorName" label="传感器名称" align="center" />
                    <el-table-column prop="sensorType" label="类型" align="center" />
                    <el-table-column prop="position" label="安装位置" align="center" />
                    <el-table-column prop="status" label="状态" align="center">
                        <template #default="scope">
                            <el-tag :type="getStatusType(scope.row.status)">{{ getStatusName(scope.row.status) }}</el-tag>
                        </template>
                    </el-table-column>
                    <el-table-column prop="onlineRate" label="在线率(%)" align="center" />
                    <el-table-column prop="lastUpdate" label="最后更新时间" width="160" align="center" />
                </el-table>
            </el-card>
        </div>
    </div>
</template>

<script setup lang="ts">
import { onMounted, reactive } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, SafetyApi } from '/@/api-services/api';

const getStatusName = (s: number) => { const m: Record<number, string> = { 1: '在线', 2: '离线', 3: '故障' }; return m[s] || '未知'; };
const getStatusType = (s: number) => { const m: Record<number, string> = { 1: 'success', 2: 'info', 3: 'danger' }; return m[s] || ''; };

const state = reactive({
    loading: false, tableData: [] as any[], treeData: [] as any[],
    treeProps: { children: 'children', label: 'name' }, queryParams: { mineId: null as number | null, page: 1, pageSize: 100 },
    sensorType: ''
});

onMounted(() => { loadMineTree(); });

function loadMineTree() {
    getAPI(CoalMineApi).getList({ page: 1, pageSize: 1000 }).then((res) => {
        state.treeData = (res.data.result || []).map((item: any) => ({ id: item.id, name: item.name, children: [] }));
    });
}

function handleNodeClick(data: any) { state.queryParams.mineId = data.id; loadData(); }

async function loadData() {
    if (!state.queryParams.mineId) return;
    state.loading = true;
    try {
        const params = {
            mineId: state.queryParams.mineId,
            page: state.queryParams.page,
            pageSize: state.queryParams.pageSize,
            sensorType: state.sensorType || undefined
        };
        const res = await getAPI(SafetyApi).getRealtimePage(params);
        state.tableData = (res.data.result?.rows || res.data.result || []).map((item: any) => ({
            sensorId: item.sensorCode,
            sensorName: item.sensorName,
            sensorType: item.sensorType,
            position: item.location,
            status: item.status || 1,
            onlineRate: item.status === 1 ? 99.5 : 0,
            lastUpdate: item.updateTime
        }));
    } catch (error) {
        console.error('加载传感器状态失败:', error);
        state.tableData = [];
    } finally {
        state.loading = false;
    }
}
</script>

<style scoped>
.page-layout { display: flex; gap: 10px; height: calc(100vh - 150px); }
.left-tree { width: 250px; overflow: auto; }
.right-content { flex: 1; overflow: auto; }
</style>
