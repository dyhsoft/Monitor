<template>
    <div class="page-layout">
        <div class="left-tree">
            <el-card shadow="hover">
                <template #header><span style="font-weight: bold;">选择煤矿</span></template>
                <el-tree :data="state.treeData" :props="state.treeProps" @node-click="handleNodeClick" node-key="id" default-expand-all highlight-current />
            </el-card>
        </div>
        <div class="right-content">
            <el-row :gutter="10">
                <el-col :span="8">
                    <el-card shadow="hover">
                        <template #header>实时报警数</template>
                        <div class="stat-number danger">{{ state.stats.alarmCount }}</div>
                    </el-card>
                </el-col>
                <el-col :span="8">
                    <el-card shadow="hover">
                        <template #header>今日报警数</template>
                        <div class="stat-number warning">{{ state.stats.todayCount }}</div>
                    </el-card>
                </el-col>
            </el-row>
            <el-card shadow="hover" style="margin-top: 10px">
                <el-table :data="state.alarmList" border stripe height="350">
                    <el-table-column type="index" label="序号" width="60" align="center" />
                    <el-table-column prop="alarmTime" label="时间" align="center" />
                    <el-table-column prop="sensorName" label="传感器" align="center" />
                    <el-table-column prop="alarmType" label="类型" align="center">
                        <template #default="scope">
                            <el-tag type="danger">{{ scope.row.alarmType }}</el-tag>
                        </template>
                    </el-table-column>
                    <el-table-column prop="value" label="值" align="center" />
                </el-table>
            </el-card>
        </div>
    </div>
</template>

<script setup lang="ts">
import { onMounted, reactive } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi } from '/@/api-services/api';

const state = reactive({
    treeData: [] as any[],
    treeProps: { children: 'children', label: 'name' },
    queryParams: { mineId: null as number | null },
    stats: { alarmCount: 2, todayCount: 8 },
    alarmList: [] as any[]
});

onMounted(() => { loadMineTree(); loadAlarm(); });

function loadMineTree() {
    getAPI(CoalMineApi).getList({ page: 1, pageSize: 1000 }).then((res) => {
        state.treeData = (res.data.result || []).map((item: any) => ({ id: item.id, name: item.name, children: [] }));
    });
}

function handleNodeClick(data: any) { state.queryParams.mineId = data.id; }

function loadAlarm() {
    state.alarmList = [
        { alarmTime: '17:55', sensorName: '液压支架压力2', alarmType: '超限报警', value: '42.8MPa' }
    ];
}
</script>

<style scoped>
.page-layout { display: flex; gap: 10px; height: calc(100vh - 150px); }
.left-tree { width: 250px; overflow: auto; }
.right-content { flex: 1; overflow: auto; }
.stat-number { font-size: 32px; font-weight: bold; text-align: center; }
.stat-number.danger { color: #f56c6c; }
.stat-number.warning { color: #e6a23c; }
</style>
