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
                    <el-form-item label="测点类型">
                        <el-select v-model="state.sensorType" placeholder="请选择类型" clearable style="width: 150px;">
                            <el-option label="水位传感器" value="waterLevel" />
                            <el-option label="流量传感器" value="flow" />
                            <el-option label="排水设备" value="drainage" />
                        </el-select>
                    </el-form-item>
                    <el-form-item><el-button type="primary" @click="loadData">查询</el-button></el-form-item>
                </el-form>
            </el-card>
            <el-card shadow="hover" style="margin-top: 10px">
                <el-table :data="state.tableData" v-loading="state.loading" border stripe height="450">
                    <el-table-column type="index" label="序号" width="60" align="center" />
                    <el-table-column prop="sensorName" label="传感器名称" min-width="120" align="center" />
                    <el-table-column prop="location" label="安装位置" min-width="150" align="center" />
                    <el-table-column prop="sensorType" label="传感器类型" width="100" align="center">
                        <template #default="scope">
                            <el-tag>{{ scope.row.sensorType || '水位传感器' }}</el-tag>
                        </template>
                    </el-table-column>
                    <el-table-column prop="waterLevel" label="水位(m)" align="center" />
                    <el-table-column prop="flow" label="流量(m³/h)" align="center" />
                    <el-table-column prop="status" label="状态" width="80" align="center">
                        <template #default="scope">
                            <el-tag :type="scope.row.status === 1 ? 'success' : 'danger'">
                                {{ scope.row.status === 1 ? '正常' : '异常' }}
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
import { CoalMineApi, WaterApi } from '/@/api-services/api';

const state = reactive({
    loading: false,
    tableData: [] as any[],
    treeData: [] as any[],
    treeProps: { children: 'children', label: 'name' },
    queryParams: { mineId: null as number | null, sensorType: '' }
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

async function loadData() {
    if (!state.queryParams.mineId) return;
    state.loading = true;
    try {
        const res = await getAPI(WaterApi).getRealtimePage({ 
            mineId: state.queryParams.mineId, 
            page: 1, 
            pageSize: 100 
        });
        state.tableData = res.data.result?.rows || res.data.result || [];
    } catch (error) {
        console.error('加载水文数据失败:', error);
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
