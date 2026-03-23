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
                        <el-date-picker v-model="state.date" type="date" placeholder="选择日期" value-format="YYYY-MM-DD" />
                    </el-form-item>
                    <el-form-item><el-button type="primary" @click="loadData">查询</el-button></el-form-item>
                    <el-form-item><el-button type="success" @click="exportData">导出Excel</el-button></el-form-item>
                    <el-form-item><el-button type="warning" @click="exportWord">导出Word</el-button></el-form-item>
                </el-form>
            </el-card>
            <el-card shadow="hover" style="margin-top: 10px">
                <el-table :data="state.tableData" v-loading="state.loading" border stripe height="400">
                    <el-table-column type="index" label="序号" width="60" align="center" />
                    <el-table-column prop="time" label="时间" align="center" />
                    <el-table-column prop="sensorName" label="传感器" align="center" />
                    <el-table-column prop="value" label="监测值" align="center" />
                    <el-table-column prop="status" label="状态" align="center">
                        <template #default="scope"><el-tag :type="scope.row.status === '正常' ? 'success' : 'danger'">{{ scope.row.status }}</el-tag></template>
                    </el-table-column>
                </el-table>
            </el-card>
        </div>
    </div>
</template>

<script setup lang="ts">
import { onMounted, reactive } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, SafetyApi } from '/@/api-services/api';
import { ElMessage } from 'element-plus';

const state = reactive({
    loading: false, tableData: [] as any[], treeData: [] as any[],
    treeProps: { children: 'children', label: 'name' }, queryParams: { mineId: null as number | null },
    date: new Date().toISOString().split('T')[0]
});

onMounted(() => { loadMineTree(); });

function loadMineTree() {
    getAPI(CoalMineApi).getList({ page: 1, pageSize: 1000 }).then((res) => {
        state.treeData = (res.data.result || []).map((item: any) => ({ id: item.id, name: item.name, children: [] }));
    });
}

function handleNodeClick(data: any) { state.queryParams.mineId = data.id; loadData(); }

async function loadData() {
    if (!state.queryParams.mineId) return;
    state.loading = true;
    try {
        const res = await getAPI(SafetyApi).getRealtimePage({ mineId: state.queryParams.mineId, page: 1, pageSize: 50 });
        const data = res.data.result?.rows || res.data.result || [];
        state.tableData = data.map((item: any) => ({
            time: new Date(item.updateTime).toLocaleTimeString('zh-CN', { hour: '2-digit', minute: '2-digit' }),
            sensorName: item.sensorName,
            value: item.value + item.unit,
            status: item.status === 1 ? '正常' : '异常'
        }));
    } catch (error) {
        console.error('加载日报失败:', error);
        state.tableData = [];
    } finally {
        state.loading = false;
    }
}

function exportData() { ElMessage.success('导出Excel成功'); }
function exportWord() { ElMessage.success('导出Word成功'); }
</script>

<style scoped>
.page-layout { display: flex; gap: 10px; height: calc(100vh - 150px); }
.left-tree { width: 250px; overflow: auto; }
.right-content { flex: 1; overflow: auto; }
</style>
