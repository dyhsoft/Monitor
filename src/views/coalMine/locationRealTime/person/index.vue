<template>
    <div class="page-layout">
        <div class="left-tree">
            <el-card shadow="hover">
                <template #header>
                    <span style="font-weight: bold;">选择煤矿</span>
                </template>
                <el-tree :data="state.treeData" :props="state.treeProps" @node-click="handleNodeClick" node-key="id" default-expand-all highlight-current />
            </el-card>
        </div>
        <div class="right-content">
            <el-card shadow="hover">
                <el-form :inline="true">
                    <el-form-item>
                        <el-button type="primary" icon="ele-Search" @click="loadData">查询</el-button>
                    </el-form-item>
                </el-form>
            </el-card>
            <el-card shadow="hover" style="margin-top: 10px">
                <el-table :data="state.tableData" v-loading="state.loading" border stripe height="500">
                    <el-table-column type="index" label="序号" width="60" align="center" />
                    <el-table-column prop="personName" label="姓名" align="center" />
                    <el-table-column prop="cardId" label="卡号" align="center" />
                    <el-table-column prop="deptName" label="部门" align="center" />
                    <el-table-column prop="stationId" label="基站" align="center" />
                    <el-table-column prop="areaName" label="区域" align="center" />
                    <el-table-column prop="x" label="坐标X" align="center" />
                    <el-table-column prop="y" label="坐标Y" align="center" />
                    <el-table-column prop="z" label="深度" align="center" />
                    <el-table-column prop="updateTime" label="更新时间" width="160" align="center" />
                </el-table>
            </el-card>
        </div>
    </div>
</template>

<script setup lang="ts">
import { onMounted, reactive } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, PersonApi } from '/@/api-services/api';

const state = reactive({
    loading: false,
    tableData: [] as any[],
    treeData: [] as any[],
    treeProps: { children: 'children', label: 'name' },
    queryParams: { mineId: null as number | null }
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
    getAPI(PersonApi).getRealTime(state.queryParams.mineId).then((res) => {
        state.tableData = res.data.result || [];
    }).finally(() => { state.loading = false; });
}
</script>

<style scoped>
.page-layout { display: flex; gap: 10px; height: calc(100vh - 150px); }
.left-tree { width: 250px; overflow: auto; }
.right-content { flex: 1; overflow: auto; }
</style>
