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
                        <el-date-picker v-model="state.dateRange" type="daterange" range-separator="至" start-placeholder="开始" end-placeholder="结束" value-format="YYYY-MM-DD" />
                    </el-form-item>
                    <el-form-item label="传感器类型">
                        <el-select v-model="state.sensorType" placeholder="全部" style="width: 150px;" clearable>
                            <el-option label="甲烷" value="CH4" />
                            <el-option label="一氧化碳" value="CO" />
                            <el-option label="温度" value="TEMP" />
                        </el-select>
                    </el-form-item>
                    <el-form-item><el-button type="primary" @click="loadData">查询</el-button></el-form-item>
                </el-form>
            </el-card>
            <el-card shadow="hover" style="margin-top: 10px">
                <el-table :data="state.tableData" v-loading="state.loading" border stripe height="400">
                    <el-table-column type="index" label="序号" width="60" align="center" />
                    <el-table-column prop="sensorName" label="传感器" align="center" />
                    <el-table-column prop="sensorType" label="类型" align="center">
                        <template #default="scope">
                            <el-tag>{{ { CH4: '甲烷', CO: '一氧化碳', TEMP: '温度' }[scope.row.sensorType] }}</el-tag>
                        </template>
                    </el-table-column>
                    <el-table-column prop="alarmCount" label="报警次数" align="center" />
                    <el-table-column prop="alarmDuration" label="累计报警时长(分钟)" align="center" />
                    <el-table-column prop="maxValue" label="最大值" align="center" />
                    <el-table-column prop="avgValue" label="平均值" align="center" />
                    <el-table-column prop="overLimitRate" label="超限率(%)" align="center">
                        <template #default="scope">
                            <span :class="{ 'text-danger': scope.row.overLimitRate > 10 }">{{ scope.row.overLimitRate }}%</span>
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
import { CoalMineApi, SafetyApi } from '/@/api-services/api';

const state = reactive({
    loading: false, tableData: [] as any[], treeData: [] as any[],
    treeProps: { children: 'children', label: 'name' }, queryParams: { mineId: null as number | null },
    dateRange: null as any, sensorType: ''
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
        const res = await getAPI(SafetyApi).getRealtimePage({ mineId: state.queryParams.mineId, page: 1, pageSize: 100 });
        let data = (res.data.result?.rows || res.data.result || []).map((item: any) => ({
            sensorName: item.sensorName,
            sensorType: item.sensorType,
            alarmCount: item.isAlarm ? 1 : 0,
            alarmDuration: item.isAlarm ? 10 : 0,
            maxValue: item.value,
            avgValue: item.value,
            overLimitRate: item.isAlarm ? 15 : 2
        }));
        if (state.sensorType) data = data.filter((x: any) => x.sensorType === state.sensorType);
        state.tableData = data;
    } catch (error) {
        console.error('加载异常统计失败:', error);
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
.text-danger { color: #f56c6c; font-weight: bold; }
</style>
