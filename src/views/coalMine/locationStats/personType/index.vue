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
                    <el-form-item label="人员类型">
                        <el-select v-model="state.personType" placeholder="请选择" style="width: 150px;">
                            <el-option label="全部" value="" />
                            <el-option label="正式工" value="1" />
                            <el-option label="临时工" value="2" />
                            <el-option label="访客" value="3" />
                        </el-select>
                    </el-form-item>
                    <el-form-item><el-button type="primary" @click="loadData">查询</el-button></el-form-item>
                </el-form>
            </el-card>
            <el-card shadow="hover" style="margin-top: 10px">
                <el-table :data="state.tableData" v-loading="state.loading" border stripe height="400">
                    <el-table-column type="index" label="序号" width="60" align="center" />
                    <el-table-column prop="personType" label="人员类型" align="center">
                        <template #default="scope">
                            <el-tag>{{ ['正式工', '临时工', '访客'][scope.row.personType - 1] }}</el-tag>
                        </template>
                    </el-table-column>
                    <el-table-column prop="totalCount" label="总人数" align="center" />
                    <el-table-column prop="inCount" label="当前在岗" align="center" />
                    <el-table-column prop="outCount" label="当前井上" align="center" />
                    <el-table-column prop="inRate" label="在岗率" align="center">
                        <template #default="scope">
                            <el-progress :percentage="scope.row.inRate" />
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
    personType: ''
);

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
        let data = [
            { personType: 1, totalCount: 200, inCount: 150, outCount: 50, inRate: 75 },
            { personType: 2, totalCount: 50, inCount: 30, outCount: 20, inRate: 60 },
            { personType: 3, totalCount: 10, inCount: 5, outCount: 5, inRate: 50 },
        ];
        if (state.personType) {
            data = data.filter((x: any) => x.personType === parseInt(state.personType));
        }
        state.tableData = data;
        state.loading = false;
    }, 300);
}
</script>

<style scoped>
.page-layout { display: flex; gap: 10px; height: calc(100vh - 150px); }
.left-tree { width: 250px; overflow: auto; }
.right-content { flex: 1; overflow: auto; }
</style>
