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
                    <el-form-item label="报表类型">
                        <el-select v-model="state.reportType" style="width: 120px;">
                            <el-option label="日报" value="day" />
                            <el-option label="月报" value="month" />
                            <el-option label="年报" value="year" />
                        </el-select>
                    </el-form-item>
                    <el-form-item label="日期">
                        <el-date-picker v-model="state.date" type="date" placeholder="选择日期" value-format="YYYY-MM-DD" />
                    </el-form-item>
                    <el-form-item><el-button type="primary" @click="loadData">查询</el-button></el-form-item>
                    <el-form-item><el-button type="success">导出</el-button></el-form-item>
                </el-form>
            </el-card>
            <el-card shadow="hover" style="margin-top: 10px">
                <el-table :data="state.tableData" v-loading="state.loading" border stripe height="400">
                    <el-table-column type="index" label="序号" width="60" align="center" />
                    <el-table-column prop="date" label="日期" align="center" />
                    <el-table-column prop="inCount" label="下井人次" align="center" />
                    <el-table-column prop="totalHours" label="累计工时" align="center" />
                    <el-table-column prop="avgDuration" label="人均时长" align="center" />
                    <el-table-column prop="maxInMine" label="最大在岗" align="center" />
                    <el-table-column prop="alarmCount" label="报警次数" align="center" />
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
    reportType: 'day', date: ''
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

function loadData() {
    if (!state.queryParams.mineId) return;
    state.loading = true;
    setTimeout(() => {
        state.tableData = [
            { date: '2026-03-01', inCount: 150, totalHours: 1200, avgDuration: 8.0, maxInMine: 85, alarmCount: 5 },
            { date: '2026-03-02', inCount: 145, totalHours: 1160, avgDuration: 8.0, maxInMine: 82, alarmCount: 3 },
            { date: '2026-03-03', inCount: 148, totalHours: 1184, avgDuration: 8.0, maxInMine: 84, alarmCount: 4 },
            { date: '2026-03-04', inCount: 152, totalHours: 1216, avgDuration: 8.0, maxInMine: 86, alarmCount: 6 },
            { date: '2026-03-05', inCount: 140, totalHours: 1120, avgDuration: 8.0, maxInMine: 80, alarmCount: 2 },
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
