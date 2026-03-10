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
                <el-table :data="state.tableData" v-loading="state.loading" border stripe height="450">
                    <el-table-column type="index" label="序号" width="60" align="center" />
                    <el-table-column prop="areaName" label="区域名称" align="center" />
                    <el-table-column prop="areaType" label="区域类型" align="center" />
                    <el-table-column prop="sensorCount" label="传感器数量" align="center" />
                    <el-table-column prop="normalCount" label="正常" align="center">
                        <template #default="scope"><span style="color: #67c23a;">{{ scope.row.normalCount }}</span></template>
                    </el-table-column>
                    <el-table-column prop="alarmCount" label="报警" align="center">
                        <template #default="scope"><span style="color: #f56c6c;">{{ scope.row.alarmCount }}</span></template>
                    </el-table-column>
                    <el-table-column prop="offlineCount" label="离线" align="center">
                        <template #default="scope"><span style="color: #909399;">{{ scope.row.offlineCount }}</span></template>
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
    treeProps: { children: 'children', label: 'name' }, queryParams: { mineId: null as number | null }
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
            { areaName: '采煤面A', areaType: '采煤面', sensorCount: 8, normalCount: 7, alarmCount: 1, offlineCount: 0 },
            { areaName: '采煤面B', areaType: '采煤面', sensorCount: 6, normalCount: 6, alarmCount: 0, offlineCount: 0 },
            { areaName: '掘进面1', areaType: '掘进面', sensorCount: 5, normalCount: 4, alarmCount: 0, offlineCount: 1 },
            { areaName: '主井', areaType: '巷道', sensorCount: 4, normalCount: 4, alarmCount: 0, offlineCount: 0 },
            { areaName: '副井', areaType: '巷道', sensorCount: 3, normalCount: 3, alarmCount: 0, offlineCount: 0 },
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
