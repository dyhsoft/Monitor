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
                    <el-form-item label="时间范围">
                        <el-date-picker v-model="state.dateRange" type="daterange" range-separator="至" start-placeholder="开始" end-placeholder="结束" value-format="YYYY-MM-DD" />
                    </el-form-item>
                    <el-form-item><el-button type="primary" @click="loadData">查询</el-button></el-form-item>
                </el-form>
            </el-card>
            <el-card shadow="hover" style="margin-top: 10px">
                <el-table :data="state.tableData" v-loading="state.loading" border stripe height="450">
                    <el-table-column type="index" label="序号" width="60" align="center" />
                    <el-table-column prop="sensorName" label="传感器名称" min-width="120" align="center" />
                    <el-table-column prop="location" label="安装位置" min-width="150" align="center" />
                    <el-table-column prop="waterLevel" label="水位(m)" align="center" />
                    <el-table-column prop="flow" label="流量(m³/h)" align="center" />
                    <el-table-column prop="recordTime" label="记录时间" width="160" align="center" />
                </el-table>
            </el-card>
        </div>
    </div>
</template>

<script setup lang="ts">
import { onMounted, reactive } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, WaterApi } from '/@/api-services/api';

const state = reactive({
    loading: false,
    tableData: [] as any[],
    treeData: [] as any[],
    treeProps: { children: 'children', label: 'name' },
    queryParams: { mineId: null as number | null, dateRange: null as any }
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
        const params: any = { mineId: state.queryParams.mineId, page: 1, pageSize: 100 };
        if (state.queryParams.dateRange && state.queryParams.dateRange.length === 2) {
            params.startTime = state.queryParams.dateRange[0];
            params.endTime = state.queryParams.dateRange[1];
        }
        const res = await getAPI(WaterApi).getHistoryPage(params);
        state.tableData = res.data.result?.rows || res.data.result || [];
    } catch (error) {
        console.error('加载水文历史数据失败:', error);
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
