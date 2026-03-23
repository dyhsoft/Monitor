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
import { CoalMineApi, SafetyApi } from '/@/api-services/api';

const state = reactive({
    loading: false, tableData: [] as any[], treeData: [] as any[],
    treeProps: { children: 'children', label: 'name' }, queryParams: { mineId: null as number | null, page: 1, pageSize: 1000 }
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
        const res = await getAPI(SafetyApi).getRealtimePage(state.queryParams);
        const data = res.data.result?.rows || res.data.result || [];
        // 按区域分组统计
        const areaMap = new Map<string, any>();
        data.forEach((item: any) => {
            const areaName = item.areaName || item.location || '未知区域';
            if (!areaMap.has(areaName)) {
                areaMap.set(areaName, { areaName, areaType: '采煤面', sensorCount: 0, normalCount: 0, alarmCount: 0, offlineCount: 0 });
            }
            const area = areaMap.get(areaName);
            area.sensorCount++;
            if (item.status === 1) area.normalCount++;
            else if (item.status === 2) area.offlineCount++;
            else if (item.isAlarm) area.alarmCount++;
        });
        state.tableData = Array.from(areaMap.values());
    } catch (error) {
        console.error('加载区域监控数据失败:', error);
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
