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
                    <el-form-item label="人员">
                        <el-input v-model="state.personName" placeholder="请输入姓名" clearable style="width: 150px;" />
                    </el-form-item>
                    <el-form-item label="日期">
                        <el-date-picker v-model="state.date" type="date" placeholder="选择日期" value-format="YYYY-MM-DD" />
                    </el-form-item>
                    <el-form-item><el-button type="primary" @click="loadData">查询</el-button></el-form-item>
                    <el-form-item><el-button type="success" :disabled="!state.trackData.length" @click="playTrack">播放轨迹</el-button></el-form-item>
                </el-form>
            </el-card>
            <el-card shadow="hover" style="margin-top: 10px">
                <div class="track-map">
                    <el-empty description="轨迹回放地图区域（待集成GIS系统）" />
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
    personName: '', date: '', trackData: [] as any[]
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
    if (!state.queryParams.mineId || !state.personName) return;
    state.trackData = [
        { x: 100, y: 200, time: '08:30', area: '主井' },
        { x: 150, y: 250, time: '09:00', area: '巷道A' },
        { x: 200, y: 300, time: '09:30', area: '采煤面A' },
        { x: 250, y: 320, time: '10:00', area: '采煤面A' },
        { x: 220, y: 310, time: '10:30', area: '采煤面A' },
    ];
}

function playTrack() {
    console.log('播放轨迹', state.trackData);
}
</script>

<style scoped>
.page-layout { display: flex; gap: 10px; height: calc(100vh - 150px); }
.left-tree { width: 250px; overflow: auto; }
.right-content { flex: 1; overflow: auto; }
.track-map { height: 450px; display: flex; align-items: center; justify-content: center; background: #f5f5f5; }
</style>
