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
                    <el-form-item label="人员">
                        <el-input v-model="state.personName" placeholder="请输入姓名" clearable style="width: 150px;" />
                    </el-form-item>
                    <el-form-item label="日期">
                        <el-date-picker v-model="state.dateRange" type="daterange" range-separator="至" start-placeholder="开始" end-placeholder="结束" value-format="YYYY-MM-DD" />
                    </el-form-item>
                    <el-form-item><el-button type="primary" @click="loadData">查询</el-button></el-form-item>
                </el-form>
            </el-card>
            <el-card shadow="hover" style="margin-top: 10px">
                <el-table :data="state.tableData" v-loading="state.loading" border stripe height="400">
                    <el-table-column type="index" label="序号" width="60" align="center" />
                    <el-table-column prop="personName" label="姓名" align="center" />
                    <el-table-column prop="areaName" label="区域" align="center" />
                    <el-table-column prop="enterTime" label="进入时间" width="160" align="center" />
                    <el-table-column prop="leaveTime" label="离开时间" width="160" align="center" />
                    <el-table-column prop="stayDuration" label="停留时长(分钟)" align="center" />
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
    personName: '', dateRange: null as any
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
            { personName: '张三', areaName: '主井', enterTime: '2026-03-09 08:30', leaveTime: '2026-03-09 09:15', stayDuration: 45 },
            { personName: '张三', areaName: '采煤面A', enterTime: '2026-03-09 09:20', leaveTime: '2026-03-09 12:00', stayDuration: 160 },
            { personName: '张三', areaName: '食堂', enterTime: '2026-03-09 12:00', leaveTime: '2026-03-09 12:30', stayDuration: 30 },
            { personName: '李四', areaName: '副井', enterTime: '2026-03-09 08:00', leaveTime: '2026-03-09 10:00', stayDuration: 120 },
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
