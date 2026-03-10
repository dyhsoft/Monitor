<template>
    <div class="page-layout">
        <div class="left-tree"><el-card shadow="hover"><template #header><span style="font-weight: bold;">选择煤矿</span></template><el-tree :data="state.treeData" :props="state.treeProps" @node-click="handleNodeClick" node-key="id" default-expand-all highlight-current /></el-card></div>
        <div class="right-content">
            <el-card shadow="hover"><el-table :data="state.tableData" v-loading="state.loading" border stripe height="400"><el-table-column type="index" label="序号" width="60" align="center" /><el-table-column prop="sensorName" label="传感器" align="center" /><el-table-column prop="faultCount" label="故障次数" align="center" /><el-table-column prop="healthScore" label="健康评分" align="center"><template #default="scope"><el-progress :percentage="scope.row.healthScore" :color="scope.row.healthScore < 60 ? '#f56c6c' : '#67c23a'" /></template></el-table-column></el-table></el-card>
        </div>
    </div>
</template>

<script setup lang="ts">
import { onMounted, reactive } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi } from '/@/api-services/api';

const state = reactive({ loading: false, tableData: [] as any[], treeData: [] as any[], treeProps: { children: 'children', label: 'name' }, queryParams: { mineId: null as number | null } });

onMounted(() => { loadMineTree(); loadData(); });
function loadMineTree() { getAPI(CoalMineApi).getList({ page: 1, pageSize: 1000 }).then((res) => { state.treeData = (res.data.result || []).map((item: any) => ({ id: item.id, name: item.name, children: [] })); }); }
function handleNodeClick(data: any) { state.queryParams.mineId = data.id; }
function loadData() { state.tableData = [{ sensorName: '液压支架压力1', faultCount: 1, healthScore: 95 }, { sensorName: '液压支架压力2', faultCount: 3, healthScore: 72 }]; }
</script>

<style scoped>
.page-layout { display: flex; gap: 10px; height: calc(100vh - 150px); }
.left-tree { width: 250px; overflow: auto; }
.right-content { flex: 1; overflow: auto; }
</style>
