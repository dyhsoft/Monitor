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
                    <el-form-item label="测点">
                        <el-select v-model="state.sensorId" placeholder="请选择测点" style="width: 200px;" clearable>
                            <el-option label="甲烷传感器1" value="CH4-001" />
                            <el-option label="甲烷传感器2" value="CH4-002" />
                            <el-option label="一氧化碳传感器" value="CO-001" />
                        </el-select>
                    </el-form-item>
                    <el-form-item label="日期">
                        <el-date-picker v-model="state.date" type="date" placeholder="选择日期" value-format="YYYY-MM-DD" />
                    </el-form-item>
                    <el-form-item><el-button type="primary" @click="loadData">查询</el-button></el-form-item>
                </el-form>
            </el-card>
            <el-card shadow="hover" style="margin-top: 10px">
                <div class="chart-area">
                    <el-empty :description="state.sensorId && state.date ? '历史曲线图表区域（待集成图表库）' : '请选择测点和日期'" />
                </div>
            </el-card>
        </div>
    </div>
</template>

<script setup lang="ts">
import { onMounted, reactive } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi } from '/@/api-services/api';

const state = reactive({
    treeData: [] as any[], treeProps: { children: 'children', label: 'name' },
    queryParams: { mineId: null as number | null },
    sensorId: '', date: '', chartData: [] as any[]
});

onMounted(() => { loadMineTree(); });

function loadMineTree() {
    getAPI(CoalMineApi).getList({ page: 1, pageSize: 1000 }).then((res) => {
        state.treeData = (res.data.result || []).map((item: any) => ({ id: item.id, name: item.name, children: [] }));
    });
}

function handleNodeClick(data: any) {
    state.queryParams.mineId = data.id;
}

function loadData() {
    if (!state.queryParams.mineId || !state.sensorId || !state.date) return;
    console.log('加载历史曲线', state.sensorId, state.date);
}
</script>

<style scoped>
.page-layout { display: flex; gap: 10px; height: calc(100vh - 150px); }
.left-tree { width: 250px; overflow: auto; }
.right-content { flex: 1; overflow: auto; }
.chart-area { height: 450px; display: flex; align-items: center; justify-content: center; background: #f9f9f9; }
</style>
