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
                <el-row :gutter="10">
                    <el-col :span="12">
                        <div class="chart-container">
                            <div style="text-align: center; padding: 20px;">井下人员曲线（人数/时间）</div>
                            <el-empty description="曲线图表区域（待集成图表库）" />
                        </div>
                    </el-col>
                    <el-col :span="12">
                        <div class="chart-container">
                            <div style="text-align: center; padding: 20px;">限定人数: {{ state.limitCount }}人</div>
                            <el-empty description="当前井下人数曲线" />
                        </div>
                    </el-col>
                </el-row>
            </el-card>
        </div>
    </div>
</template>

<script setup lang="ts">
import { onMounted, reactive } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, PersonApi, LocationLimitConfigApi } from '/@/api-services/api';

const state = reactive({
    treeData: [] as any[], treeProps: { children: 'children', label: 'name' },
    queryParams: { mineId: null as number | null },
    limitCount: 0
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
    getAPI(PersonApi).getRealTime(state.queryParams.mineId).then((res) => {
        const count = (res.data.result || []).length;
        console.log('当前人数:', count);
    });
    getAPI(LocationLimitConfigApi).getPage({ mineId: state.queryParams.mineId, page: 1, pageSize: 1 }).then((res) => {
        state.limitCount = (res.data.result?.items || [])[0]?.limitCount || 0;
    });
}
</script>

<style scoped>
.page-layout { display: flex; gap: 10px; height: calc(100vh - 150px); }
.left-tree { width: 250px; overflow: auto; }
.right-content { flex: 1; overflow: auto; }
.chart-container { height: 400px; background: #f9f9f9; }
</style>
