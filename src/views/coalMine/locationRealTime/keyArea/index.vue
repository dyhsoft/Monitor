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
                    <el-form-item label="区域类型">
                        <el-select v-model="state.areaType" placeholder="请选择" style="width: 150px;">
                            <el-option label="全部" value="" />
                            <el-option label="采煤面" value="1" />
                            <el-option label="掘进面" value="2" />
                            <el-option label="巷道" value="3" />
                        </el-select>
                    </el-form-item>
                    <el-form-item><el-button type="primary" @click="loadData">查询</el-button></el-form-item>
                </el-form>
            </el-card>
            <el-card shadow="hover" style="margin-top: 10px">
                <el-table :data="state.tableData" v-loading="state.loading" border stripe height="400">
                    <el-table-column type="index" label="序号" width="60" align="center" />
                    <el-table-column prop="areaName" label="区域名称" align="center" />
                    <el-table-column prop="areaType" label="区域类型" align="center">
                        <template #default="scope">
                            <el-tag>{{ ['采煤面', '掘进面', '巷道'][scope.row.areaType - 1] || '未知' }}</el-tag>
                        </template>
                    </el-table-column>
                    <el-table-column prop="personCount" label="当前人数" align="center" />
                    <el-table-column prop="limitCount" label="限定人数" align="center" />
                    <el-table-column prop="status" label="状态" align="center">
                        <template #default="scope">
                            <el-tag :type="scope.row.personCount > scope.row.limitCount ? 'danger' : 'success'">
                                {{ scope.row.personCount > scope.row.limitCount ? '超员' : '正常' }}
                            </el-tag>
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
    areaType: ''
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
        let data = [
            { areaName: '采煤面A', areaType: 1, personCount: 18, limitCount: 20 },
            { areaName: '采煤面B', areaType: 1, personCount: 15, limitCount: 18 },
            { areaName: '掘进面1', areaType: 2, personCount: 8, limitCount: 10 },
            { areaName: '掘进面2', areaType: 2, personCount: 6, limitCount: 10 },
        ];
        if (state.areaType) {
            data = data.filter((x: any) => x.areaType === parseInt(state.areaType));
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
