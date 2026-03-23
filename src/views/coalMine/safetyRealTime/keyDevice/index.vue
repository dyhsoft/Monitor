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
                    <el-table-column prop="deviceName" label="设备名称" align="center" />
                    <el-table-column prop="deviceType" label="设备类型" align="center" />
                    <el-table-column prop="location" label="位置" align="center" />
                    <el-table-column prop="status" label="运行状态" align="center">
                        <template #default="scope">
                            <el-tag :type="scope.row.status === 1 ? 'success' : 'danger'">{{ scope.row.status === 1 ? '正常运行' : '异常' }}</el-tag>
                        </template>
                    </el-table-column>
                    <el-table-column prop="value" label="监测值" align="center" />
                    <el-table-column prop="updateTime" label="更新时间" width="160" align="center" />
                </el-table>
            </el-card>
        </div>
    </div>
</template>

<script setup lang="ts">
import { onMounted, reactive } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, SafetyApi } from '/@/api-services/api';

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

async function loadData() {
    if (!state.queryParams.mineId) return;
    state.loading = true;
    try {
        const res = await getAPI(SafetyApi).getRealtimePage({ mineId: state.queryParams.mineId, page: 1, pageSize: 50 });
        state.tableData = (res.data.result?.rows || res.data.result || []).map((item: any) => ({
            deviceName: item.sensorName,
            deviceType: item.sensorType,
            location: item.location,
            status: item.status === 1 ? 1 : 2,
            value: item.value,
            updateTime: item.updateTime
        }));
    } catch (error) {
        console.error('加载重点设备数据失败:', error);
        state.tableData = [];
    } finally {
        state.loading = false;
    }
}
</script>

<style scoped>
.page-layout { display: flex; gap: 10px; height: calc(100vh - 150px); }
.left-tree { width: 250px; overflow: auto; }
.right-content { flex: 1; overflow: auto; }
</style>
