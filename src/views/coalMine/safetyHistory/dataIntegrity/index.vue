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
import { CoalMineApi, SafetyApi } from '/@/api-services/api';

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

async function loadData() {
    if (!state.queryParams.mineId) return;
    state.loading = true;
    try {
        const res = await getAPI(SafetyApi).getRealtimePage({ mineId: state.queryParams.mineId, page: 1, pageSize: 100 });
        const data = res.data.result?.rows || res.data.result || [];
        const total = data.length;
        const missing = data.filter((d: any) => d.status !== 1).length;
        state.tableData = [{
            date: new Date().toISOString().split('T')[0],
            totalPoints: total,
            actualPoints: total - missing,
            completeRate: total > 0 ? ((total - missing) / total * 100).toFixed(1) : 100,
            missingPoints: missing,
            missingDuration: missing * 5
        }];
    } catch (error) {
        console.error('加载数据完整性失败:', error);
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
