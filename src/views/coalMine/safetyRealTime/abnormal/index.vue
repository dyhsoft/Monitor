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
                    <el-form-item label="报警类型">
                        <el-select v-model="state.alarmType" placeholder="全部" style="width: 150px;" clearable>
                            <el-option label="超限报警" value="1" />
                            <el-option label="断电报警" value="2" />
                            <el-option label="故障报警" value="3" />
                        </el-select>
                    </el-form-item>
                    <el-form-item><el-button type="primary" @click="loadData">查询</el-button></el-form-item>
                </el-form>
            </el-card>
            <el-card shadow="hover" style="margin-top: 10px">
                <el-table :data="state.tableData" v-loading="state.loading" border stripe height="400">
                    <el-table-column type="index" label="序号" width="60" align="center" />
                    <el-table-column prop="sensorId" label="传感器编号" align="center" />
                    <el-table-column prop="sensorName" label="传感器名称" align="center" />
                    <el-table-column prop="alarmType" label="报警类型" align="center">
                        <template #default="scope">
                            <el-tag :type="getAlarmTypeColor(scope.row.alarmType)">{{ getAlarmTypeName(scope.row.alarmType) }}</el-tag>
                        </template>
                    </el-table-column>
                    <el-table-column prop="alarmValue" label="报警值" align="center" />
                    <el-table-column prop="limitValue" label="限值" align="center" />
                    <el-table-column prop="position" label="位置" align="center" />
                    <el-table-column prop="alarmTime" label="报警时间" width="160" align="center" />
                    <el-table-column prop="status" label="状态" align="center">
                        <template #default="scope">
                            <el-tag :type="scope.row.status === 1 ? 'danger' : 'success'">
                                {{ scope.row.status === 1 ? '未处理' : '已恢复' }}
                            </el-tag>
                        </template>
                    </el-table-column>
                </el-table>
            </el-card>
        </div>
    </div>
</template>

<script setup lang="ts">
import { onMounted, reactive } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi } from '/@/api-services/api';

const getAlarmTypeName = (type: number) => { const map = { 1: '超限报警', 2: '断电报警', 3: '故障报警' }; return map[type] || '未知'; };
const getAlarmTypeColor = (type: number) => { const map = { 1: 'danger', 2: 'warning', 3: 'info' }; return map[type] || ''; };

const state = reactive({
    loading: false, tableData: [] as any[], treeData: [] as any[],
    treeProps: { children: 'children', label: 'name' }, queryParams: { mineId: null as number | null },
    alarmType: ''
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
    state.loading = true;
    setTimeout(() => {
        let data = [
            { sensorId: 'CH4-002', sensorName: '甲烷传感器2', alarmType: 1, alarmValue: 0.85, limitValue: 0.5, position: '掘进面1', alarmTime: '2026-03-09 14:30:00', status: 1 },
            { sensorId: 'CH4-003', sensorName: '甲烷传感器3', alarmType: 2, alarmValue: 1.2, limitValue: 0.5, position: '采煤面B', alarmTime: '2026-03-09 13:20:00', status: 2 },
            { sensorId: 'CO-002', sensorName: '一氧化碳传感器2', alarmType: 1, alarmValue: 35, limitValue: 24, position: '中央泵房', alarmTime: '2026-03-09 12:10:00', status: 1 },
            { sensorId: 'TEMP-002', sensorName: '温度传感器2', alarmType: 3, alarmValue: '--', limitValue: '--', position: '采煤面A', alarmTime: '2026-03-09 11:00:00', status: 2 },
        ];
        if (state.alarmType) data = data.filter((x: any) => x.alarmType === parseInt(state.alarmType));
        state.tableData = data;
        state.loading = false;
    }, 300);
}
</script>

<style scoped>
.page-layout { display: flex; gap: 10px; height: calc(100vh - 150px); }
.left-tree { width: 250px; overflow: auto; }
.right-content { flex: 1; overflow: auto; }
</style>
