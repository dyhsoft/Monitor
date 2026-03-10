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
                    <el-table-column prop="stationId" label="基站编号" align="center" />
                    <el-table-column prop="stationName" label="基站名称" align="center" />
                    <el-table-column prop="areaName" label="区域" align="center" />
                    <el-table-column prop="x" label="坐标X" align="center" />
                    <el-table-column prop="y" label="坐标Y" align="center" />
                    <el-table-column prop="status" label="状态" width="80" align="center">
                        <template #default="scope">
                            <el-tag :type="scope.row.status === 1 ? 'success' : 'danger'">
                                {{ scope.row.status === 1 ? '在线' : '离线' }}
                            </el-tag>
                        </template>
                    </el-table-column>
                    <el-table-column prop="updateTime" label="更新时间" width="160" align="center" />
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
    // 模拟基站数据
    setTimeout(() => {
        state.tableData = [
            { stationId: 'ST001', stationName: '主井基站', areaName: '主井', x: 100, y: 200, status: 1, updateTime: new Date().toLocaleString() },
            { stationId: 'ST002', stationName: '副井基站', areaName: '副井', x: 150, y: 250, status: 1, updateTime: new Date().toLocaleString() },
            { stationId: 'ST003', stationName: '东巷基站', areaName: '东巷', x: 200, y: 300, status: 0, updateTime: new Date().toLocaleString() },
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
