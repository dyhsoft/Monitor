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
                            <el-option label="风速" value="WIND" />
                            <el-option label="负压" value="PRESS" />
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
                    <el-table-column prop="sensorType" label="类型" align="center">
                        <template #default="scope">
                            <el-tag>{{ sensorTypeMap[scope.row.sensorType] || scope.row.sensorType }}</el-tag>
                        </template>
                    </el-table-column>
                    <el-table-column prop="value" label="监测值" align="center">
                        <template #default="scope">
                            <span :class="{ 'text-danger': scope.row.isAlarm }">{{ scope.row.value }}{{ scope.row.unit }}</span>
                        </template>
                    </el-table-column>
                    <el-table-column prop="position" label="安装位置" align="center" />
                    <el-table-column prop="updateTime" label="更新时间" width="160" align="center" />
                </el-table>
            </el-card>
        </div>
    </div>
</template>

<script setup lang="ts">
import { onMounted, reactive } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, SafetyApi } from '/@/api-services/api';

const sensorTypeMap: Record<string, string> = { CH4: '甲烷', CO: '一氧化碳', TEMP: '温度', WIND: '风速', PRESS: '负压' };

const state = reactive({
    loading: false, tableData: [] as any[], treeData: [] as any[],
    treeProps: { children: 'children', label: 'name' }, queryParams: { mineId: null as number | null, page: 1, pageSize: 100, sensorType: '' },
});

onMounted(() => { loadMineTree(); });

function loadMineTree() {
    getAPI(CoalMineApi).getList({ page: 1, pageSize: 1000 }).then((res) => {
        state.treeData = (res.data.result || []).map((item: any) => ({ id: item.id, name: item.name, children: [] }));
    });
}

function handleNodeClick(data: any) {
    state.queryParams.mineId = data.id;
    loadData();
}

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
        state.tableData = res.data.result?.rows || res.data.result || [];
    } catch (error) {
        console.error('加载安全监测数据失败:', error);
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
.text-danger { color: #f56c6c; font-weight: bold; }
</style>
