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
                    <el-form-item><el-button type="primary" @click="loadData">查询</el-button></el-form-item>
                </el-form>
            </el-card>
            <el-row :gutter="10" style="margin-top: 10px;">
                <el-col :span="6">
                    <el-card shadow="hover">
                        <template #header>总报警次数</template>
                        <div class="stat-number">{{ state.stats.totalAlarm }}</div>
                    </el-card>
                </el-col>
                <el-col :span="6">
                    <el-card shadow="hover">
                        <template #header>超限报警</template>
                        <div class="stat-number danger">{{ state.stats.overLimit }}</div>
                    </el-card>
                </el-col>
                <el-col :span="6">
                    <el-card shadow="hover">
                        <template #header>断电报警</template>
                        <div class="stat-number warning">{{ state.stats.powerOff }}</div>
                    </el-card>
                </el-col>
                <el-col :span="6">
                    <el-card shadow="hover">
                        <template #header>故障报警</template>
                        <div class="stat-number info">{{ state.stats.fault }}</div>
                    </el-card>
                </el-col>
            </el-row>
            <el-card shadow="hover" style="margin-top: 10px">
                <el-table :data="state.tableData" v-loading="state.loading" border stripe height="300">
                    <el-table-column type="index" label="序号" width="60" align="center" />
                    <el-table-column prop="date" label="日期" align="center" />
                    <el-table-column prop="totalAlarm" label="总报警次数" align="center" />
                    <el-table-column prop="overLimit" label="超限" align="center" />
                    <el-table-column prop="powerOff" label="断电" align="center" />
                    <el-table-column prop="fault" label="故障" align="center" />
                    <el-table-column prop="alarmRate" label="报警率(%)" align="center" />
                </el-table>
            </el-card>
        </div>
    </div>
</template>

<script setup lang="ts">
import { onMounted, reactive } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, SafetyApi, AlarmRecordApi } from '/@/api-services/api';

const state = reactive({
    loading: false, tableData: [] as any[], treeData: [] as any[],
    treeProps: { children: 'children', label: 'name' }, queryParams: { mineId: null as number | null },
    dateRange: null as any,
    stats: { totalAlarm: 0, overLimit: 0, powerOff: 0, fault: 0 }
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
        const params: any = { mineId: state.queryParams.mineId, page: 1, pageSize: 100 };
        if (state.dateRange && state.dateRange.length === 2) {
            params.startTime = state.dateRange[0];
            params.endTime = state.dateRange[1];
        }
        const res = await getAPI(AlarmRecordApi).getPage(params);
        const data = res.data.result?.rows || res.data.result || [];
        // 统计报警类型
        let overLimit = 0, powerOff = 0, fault = 0;
        data.forEach((d: any) => {
            if (d.alarmType === 1) overLimit++;
            else if (d.alarmType === 2) powerOff++;
            else fault++;
        });
        state.tableData = [{ 
            date: state.dateRange?.[0] || new Date().toISOString().split('T')[0], 
            totalAlarm: data.length, overLimit, powerOff, fault, 
            alarmRate: ((data.length / 100) * 100).toFixed(1) 
        }];
        state.stats.totalAlarm = data.length;
        state.stats.overLimit = overLimit;
        state.stats.powerOff = powerOff;
        state.stats.fault = fault;
    } catch (error) {
        console.error('加载报警统计失败:', error);
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
.stat-number { font-size: 28px; font-weight: bold; text-align: center; }
.stat-number.danger { color: #f56c6c; }
.stat-number.warning { color: #e6a23c; }
.stat-number.info { color: #909399; }
</style>
