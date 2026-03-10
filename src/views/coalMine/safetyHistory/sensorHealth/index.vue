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
                    <el-form-item label="日期">
                        <el-date-picker v-model="state.dateRange" type="daterange" range-separator="至" start-placeholder="开始" end-placeholder="结束" value-format="YYYY-MM-DD" />
                    </el-form-item>
                    <el-form-item><el-button type="primary" @click="loadData">查询</el-button></el-form-item>
                </el-form>
            </el-card>
            <el-card shadow="hover" style="margin-top: 10px">
                <el-table :data="state.tableData" v-loading="state.loading" border stripe height="400">
                    <el-table-column type="index" label="序号" width="60" align="center" />
                    <el-table-column prop="sensorId" label="传感器编号" align="center" />
                    <el-table-column prop="sensorName" label="传感器名称" align="center" />
                    <el-table-column prop="faultCount" label="故障次数" align="center" />
                    <el-table-column prop="faultDuration" label="累计故障时长(小时)" align="center" />
                    <el-table-column prop="driftValue" label="漂移值" align="center" />
                    <el-table-column prop="healthScore" label="健康评分" align="center">
                        <template #default="scope">
                            <el-progress :percentage="scope.row.healthScore" :color="scope.row.healthScore < 60 ? '#f56c6c' : scope.row.healthScore < 80 ? '#e6a23c' : '#67c23a'" />
                        </template>
                    </el-table-column>
                </el-table>
            </el-card>
        </div>
    </div>
</template>

<script setup lang="ts">
import { onMounted, reactive } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi } from '/@/api-services/api';

const state = reactive({
    loading: false, tableData: [] as any[], treeData: [] as any[],
    treeProps: { children: 'children', label: 'name' }, queryParams: { mineId: null as number | null },
    dateRange: null as any
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
        state.tableData = [
            { sensorId: 'CH4-001', sensorName: '甲烷传感器1', faultCount: 1, faultDuration: 2, driftValue: '0.01%', healthScore: 95 },
            { sensorId: 'CH4-002', sensorName: '甲烷传感器2', faultCount: 3, faultDuration: 8, driftValue: '0.05%', healthScore: 72 },
            { sensorId: 'CO-001', sensorName: '一氧化碳传感器', faultCount: 0, faultDuration: 0, driftValue: '0', healthScore: 98 },
            { sensorId: 'TEMP-001', sensorName: '温度传感器', faultCount: 2, faultDuration: 5, driftValue: '0.5℃', healthScore: 85 },
        ];
        state.loading = false;
    }, 300);
}
</script>

<style scoped>
.page-layout { display: flex; gap: 10px; height: calc(100vh - 150px); }
.left-tree { width: 250px; overflow: auto; }
.right-content { flex: 1; overflow: auto; }
</style>
