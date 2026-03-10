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
                    <el-form-item label="报表类型">
                        <el-select v-model="state.reportType" style="width: 120px;">
                            <el-option label="日报" value="day" />
                            <el-option label="月报" value="month" />
                            <el-option label="年报" value="year" />
                        </el-select>
                    </el-form-item>
                    <el-form-item><el-button type="primary" @click="loadData">查询</el-button></el-form-item>
                </el-form>
            </el-card>
            <el-card shadow="hover" style="margin-top: 10px">
                <el-table :data="state.tableData" v-loading="state.loading" border stripe height="400">
                    <el-table-column type="index" label="序号" width="60" align="center" />
                    <el-table-column prop="personName" label="姓名" align="center" />
                    <el-table-column prop="deptName" label="部门" align="center" />
                    <el-table-column prop="inCount" label="下井次数" align="center" />
                    <el-table-column prop="totalHours" label="累计时长(小时)" align="center" />
                    <el-table-column prop="normalCount" label="正常出勤" align="center" />
                    <el-table-column prop="lateCount" label="迟到" align="center" />
                    <el-table-column prop="leaveEarlyCount" label="早退" align="center" />
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
    dateRange: null as any, reportType: 'day'
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
            { personName: '张三', deptName: '采煤队', inCount: 25, totalHours: 200, normalCount: 22, lateCount: 2, leaveEarlyCount: 1 },
            { personName: '李四', deptName: '掘进队', inCount: 24, totalHours: 192, normalCount: 24, lateCount: 0, leaveEarlyCount: 0 },
            { personName: '王五', deptName: '机电队', inCount: 22, totalHours: 176, normalCount: 20, lateCount: 1, leaveEarlyCount: 1 },
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
