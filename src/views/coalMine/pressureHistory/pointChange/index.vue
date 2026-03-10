<template>
    <div class="page-layout">
        <div class="left-tree"><el-card shadow="hover"><template #header><span style="font-weight: bold;">选择煤矿</span></template><el-tree :data="state.treeData" :props="state.treeProps" @node-click="handleNodeClick" node-key="id" default-expand-all highlight-current /></el-card></div>
        <div class="right-content">
            <el-card shadow="hover"><el-table :data="state.tableData" v-loading="state.loading" border stripe height="400"><el-table-column type="index" label="序号" width="60" align="center" /><el-table-column prop="sensorId" label="传感器编号" align="center" /><el-table-column prop="changeType" label="变更类型" align="center"><template #default="scope"><el-tag>{{ ['新增', '删除', '修改'][scope.row.changeType - 1] }}</el-tag></template></el-table-column><el-table-column prop="changeTime" label="变更时间" align="center" /><el-table-column prop="operator" label="操作人" align="center" /></el-table></el-card>
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
function loadData() { state.tableData = [{ sensorId: 'P-003', changeType: 1, changeTime: '2026-03-09 10:00:00', operator: '张三' }]; }
</script>

<style scoped>
.page-layout { display: flex; gap: 10px; height: calc(100vh - 150px); }
.left-tree { width: 250px; overflow: auto; }
.right-content { flex: 1; overflow: auto; }
</style>
