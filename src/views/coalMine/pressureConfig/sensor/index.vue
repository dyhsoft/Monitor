<template>
    <div class="page-layout">
        <div class="left-tree"><el-card shadow="hover"><template #header><span style="font-weight: bold;">选择煤矿</span></template><el-tree :data="state.treeData" :props="state.treeProps" @node-click="handleNodeClick" node-key="id" default-expand-all highlight-current /></el-card></div>
        <div class="right-content">
            <el-card shadow="hover"><el-form :inline="true"><el-form-item><el-button type="primary" icon="ele-Plus" @click="openAdd">新增传感器</el-button></el-form-item></el-form></el-card>
            <el-card shadow="hover" style="margin-top: 10px"><el-table :data="state.tableData" v-loading="state.loading" border stripe><el-table-column type="index" label="序号" width="60" align="center" /><el-table-column prop="sensorId" label="传感器编号" align="center" /><el-table-column prop="sensorName" label="传感器名称" align="center" /><el-table-column prop="enabled" label="状态" align="center"><template #default="scope"><el-switch v-model="scope.row.enabled" /></template></el-table-column></el-table></el-card>
        </div>
    </div>
</template>

<script setup lang="ts">
import { onMounted, reactive } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi } from '/@/api-services/api';
import { ElMessage } from 'element-plus';

const state = reactive({ loading: false, tableData: [] as any[], treeData: [] as any[], treeProps: { children: 'children', label: 'name' }, queryParams: { mineId: null as number | null } });

onMounted(() => { loadMineTree(); loadData(); });
function loadMineTree() { getAPI(CoalMineApi).getList({ page: 1, pageSize: 1000 }).then((res) => { state.treeData = (res.data.result || []).map((item: any) => ({ id: item.id, name: item.name, children: [] })); }); }
function handleNodeClick(data: any) { state.queryParams.mineId = data.id; }
function loadData() { state.tableData = [{ sensorId: 'P-001', sensorName: '液压支架压力1', enabled: true }]; }
function openAdd() { if (!state.queryParams.mineId) { ElMessage.warning('请先选择煤矿'); return; } ElMessage.info('新增传感器'); }
</script>

<style scoped>
.page-layout { display: flex; gap: 10px; height: calc(100vh - 150px); }
.left-tree { width: 250px; overflow: auto; }
.right-content { flex: 1; overflow: auto; }
</style>
