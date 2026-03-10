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
import { CoalMineApi } from '/@/api-services/api';

const getStatusName = (s: number) => { const m = { 1: '在线', 2: '离线', 3: '故障' }; return m[s] || '未知'; };
const getStatusType = (s: number) => { const m = { 1: 'success', 2: 'info', 3: 'danger' }; return m[s] || ''; };

const state = reactive({
    loading: false, tableData: [] as any[], treeData: [] as any[],
    treeProps: { children: 'children', label: 'name' }, queryParams: { mineId: null as number | null },
    sensorType: ''
});

onMounted(() => { loadMineTree(); });

function loadMineTree() {
    getAPI(CoalMineApi).getList({ page: 1, pageSize: 1000 }).then((res) => {
        state.treeData = (res.data.result || []).map((item: any) => ({ id: item.id, name: item.name, children: [] }));
    });
}

function handleNodeClick(data: any) { state.queryParams.mineId = data.id; loadData(); }

function loadData() {
    if (!state.queryParams.mineId) return;
    state.loading = true;
    setTimeout(() => {
        let data = [
            { sensorId: 'CH4-001', sensorName: '甲烷传感器1', sensorType: 'CH4', position: '采煤面A', status: 1, onlineRate: 99.5, lastUpdate: new Date().toLocaleString() },
            { sensorId: 'CH4-002', sensorName: '甲烷传感器2', sensorType: 'CH4', position: '掘进面1', status: 2, onlineRate: 0, lastUpdate: new Date().toLocaleString() },
            { sensorId: 'CO-001', sensorName: '一氧化碳传感器', sensorType: 'CO', position: '中央变电所', status: 1, onlineRate: 98.8, lastUpdate: new Date().toLocaleString() },
            { sensorId: 'TEMP-001', sensorName: '温度传感器', sensorType: 'TEMP', position: '采煤面A', status: 3, onlineRate: 85.0, lastUpdate: new Date().toLocaleString() },
        ];
        if (state.sensorType) data = data.filter((x: any) => x.sensorType === state.sensorType);
        state.tableData = data;
        state.loading = false;
    }, 300);
}
</script>

<style scoped>
.page-layout { display: flex; gap: 10px; height: calc(100vh - 150px); }
.left-tree { width: 250px; overflow: auto; }
.right-content { flex: 1; overflow: auto; }
</style>
