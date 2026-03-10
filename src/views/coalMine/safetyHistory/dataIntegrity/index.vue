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
                    <el-table-column prop="date" label="日期" align="center" />
                    <el-table-column prop="totalPoints" label="应采点数" align="center" />
                    <el-table-column prop="actualPoints" label="实采点数" align="center" />
                    <el-table-column prop="completeRate" label="完整率(%)" align="center">
                        <template #default="scope">
                            <el-progress :percentage="scope.row.completeRate" :color="scope.row.completeRate < 95 ? '#f56c6c' : '#67c23a'" />
                        </template>
                    </el-table-column>
                    <el-table-column prop="missingPoints" label="缺失点数" align="center" />
                    <el-table-column prop="missingDuration" label="缺失时长(分钟)" align="center" />
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
            { date: '2026-03-09', totalPoints: 1000, actualPoints: 985, completeRate: 98.5, missingPoints: 15, missingDuration: 45 },
            { date: '2026-03-08', totalPoints: 1000, actualPoints: 992, completeRate: 99.2, missingPoints: 8, missingDuration: 20 },
            { date: '2026-03-07', totalPoints: 1000, actualPoints: 978, completeRate: 97.8, missingPoints: 22, missingDuration: 65 },
            { date: '2026-03-06', totalPoints: 1000, actualPoints: 995, completeRate: 99.5, missingPoints: 5, missingDuration: 12 },
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
