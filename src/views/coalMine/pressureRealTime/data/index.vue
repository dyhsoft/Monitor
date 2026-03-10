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
                    <el-form-item label="传感器类型">
                        <el-select v-model="state.sensorType" placeholder="全部" style="width: 150px;" clearable>
                            <el-option label="压力传感器" value="PRESS" />
                            <el-option label="位移传感器" value="DISP" />
                            <el-option label="锚杆应力" value="ANCHOR" />
                        </el-select>
                    </el-form-item>
                    <el-form-item><el-button type="primary"> @click="loadData">查询</el-button></el-form-item>
                </el-form>
            </el-card>
            <el-card shadow="hover" style="margin-top: 10px">
                <el-table :data="state.tableData" v-loading="state.loading" border stripe height="400">
                    <el-table-column type="index" label="序号" width="60" align="center" />
                    <el-table-column prop="sensorId" label="传感器编号" align="center" />
                    <el-table-column prop="sensorName" label="传感器名称" align="center" />
                    <el-table-column prop="value" label="压力值(MPa)" align="center">
                        <template #default="scope"><span :class="{ 'text-danger': scope.row.isAlarm }">{{ scope.row.value }}</span></template>
                    </el-table-column>
                    <el-table-column prop="position" label="安装位置" align="center" />
                    <el-table-column prop="updateTime" label="更新时间" width="160" align="center" />
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
    loading: false, tableData: [] as any[], treeData: [] as any[],
    treeProps: { children: 'children', label: 'name' }, queryParams: { mineId: null as number | null },
    sensorType: ''
});

onMounted(() => { loadMineTree(); });

function loadMineTree() {
    getAPI(CoalMineApi).getList({ page: 1, pageSize: 1000 }).then((res) => {
        state.treeData = (res.data.result || []).map((item: any) => ({ id: item.id, name: item.name, children: [] }));
    });
}

function handleNodeClick(data: any) { state.queryParams.mineId = data.id; loadData(); }

function loadData() {
    if (!state.queryParams.mineId) return;
    state.loading = true;
    setTimeout(() => {
        let data = [
            { sensorId: 'P-001', sensorName: '液压支架压力1', value: 35.5, position: '采煤面A', isAlarm: false, updateTime: new Date().toLocaleString() },
            { sensorId: 'P-002', sensorName: '液压支架压力2', value: 42.8, position: '采煤面A', isAlarm: true, updateTime: new Date().toLocaleString() },
            { sensorId: 'P-003', sensorName: '顶板压力传感器', value: 28.3, position: '掘进面1', isAlarm: false, updateTime: new Date().toLocaleString() },
        ];
        if (state.sensorType) data = data.filter((x: any) => x.sensorType === state.sensorType);
        state.tableData = data;
        state.loading = false;
    }, 300);
}
</script>

<style scoped>
.page-layout { display: flex; gap: 10px; height: calc(100vh - 150px); }
.left-tree { width: 250px; overflow: auto; }
.right-content { flex: 1; overflow: auto; }
.text-danger { color: #f56c6c; font-weight: bold; }
</style>
