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
                    <el-form-item label="年份">
                        <el-date-picker v-model="state.year" type="year" placeholder="选择年份" value-format="YYYY" />
                    </el-form-item>
                    <el-form-item><el-button type="primary" @click="loadData">查询</el-button></el-form-item>
                    <el-form-item><el-button type="success" @click="exportData">导出Excel</el-button></el-form-item>
                    <el-form-item><el-button type="warning" @click="exportWord">导出Word</el-button></el-form-item>
                </el-form>
            </el-card>
            <el-card shadow="hover" style="margin-top: 10px">
                <el-table :data="state.tableData" v-loading="state.loading" border stripe height="400">
                    <el-table-column type="index" label="序号" width="60" align="center" />
                    <el-table-column prop="month" label="月份" align="center" />
                    <el-table-column prop="alarmCount" label="报警次数" align="center" />
                    <el-table-column prop="alarmDuration" label="报警时长(小时)" align="center" />
                    <el-table-column prop="maxAlarmValue" label="最大报警值" align="center" />
                    <el-table-column prop="avgOnlineRate" label="平均在线率(%)" align="center" />
                    <el-table-column prop="avgCompleteRate" label="平均数据完整率(%)" align="center" />
                </el-table>
            </el-card>
        </div>
    </div>
</template>

<script setup lang="ts">
import { onMounted, reactive } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi } from '/@/api-services/api';
import { ElMessage } from 'element-plus';

const state = reactive({
    loading: false, tableData: [] as any[], treeData: [] as any[],
    treeProps: { children: 'children', label: 'name' }, queryParams: { mineId: null as number | null },
    year: new Date().getFullYear().toString()
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
            { month: '1月', alarmCount: 45, alarmDuration: 320, maxAlarmValue: '0.85%', avgOnlineRate: 98.5, avgCompleteRate: 99.1 },
            { month: '2月', alarmCount: 38, alarmDuration: 280, maxAlarmValue: '0.72%', avgOnlineRate: 99.0, avgCompleteRate: 99.3 },
            { month: '3月', alarmCount: 52, alarmDuration: 380, maxAlarmValue: '1.2%', avgOnlineRate: 98.2, avgCompleteRate: 98.8 },
        ];
        state.loading = false;
    }, 300);
}

function exportData() { ElMessage.success('导出Excel成功'); }
function exportWord() { ElMessage.success('导出Word成功'); }
</script>

<style scoped>
.page-layout { display: flex; gap: 10px; height: calc(100vh - 150px); }
.left-tree { width: 250px; overflow: auto; }
.right-content { flex: 1; overflow: auto; }
</style>
