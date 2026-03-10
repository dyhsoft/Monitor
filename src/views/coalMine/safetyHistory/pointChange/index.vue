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
                    <el-form-item label="变更类型">
                        <el-select v-model="state.changeType" placeholder="全部" style="width: 150px;" clearable>
                            <el-option label="新增" value="1" />
                            <el-option label="删除" value="2" />
                            <el-option label="修改" value="3" />
                            <el-option label="检修" value="4" />
                        </el-select>
                    </el-form-item>
                    <el-form-item label="日期">
                        <el-date-picker v-model="state.dateRange" type="daterange" range-separator="至" start-placeholder="开始" end-placeholder="结束" value-format="YYYY-MM-DD" />
                    </el-form-item>
                    <el-form-item><el-button type="primary" @click="loadData">查询</el-button></el-form-item>
                </el-form>
            </el-card>
            <el-card shadow="hover" style="margin-top: 10px">
                <el-table :data="state.tableData" v-loading="state.loading" border stripe height="400">
                    <el-table-column type="index" label="序号" width="60" align="center" />
                    <el-table-column prop="sensorId" label="传感器编号" align="center" />
                    <el-table-column prop="sensorName" label="传感器名称" align="center" />
                    <el-table-column prop="changeType" label="变更类型" align="center">
                        <template #default="scope">
                            <el-tag :type="getTypeColor(scope.row.changeType)">{{ getTypeName(scope.row.changeType) }}</el-tag>
                        </template>
                    </el-table-column>
                    <el-table-column prop="oldValue" label="原值" align="center" />
                    <el-table-column prop="newValue" label="新值" align="center" />
                    <el-table-column prop="operator" label="操作人" align="center" />
                    <el-table-column prop="changeTime" label="变更时间" width="160" align="center" />
                    <el-table-column prop="reason" label="变更原因" align="center" />
                </el-table>
            </el-card>
        </div>
    </div>
</template>

<script setup lang="ts">
import { onMounted, reactive } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi } from '/@/api-services/api';

const getTypeName = (type: number) => { const map = { 1: '新增', 2: '删除', 3: '修改', 4: '检修' }; return map[type] || '未知'; };
const getTypeColor = (type: number) => { const map = { 1: 'success', 2: 'danger', 3: 'warning', 4: 'info' }; return map[type] || ''; };

const state = reactive({
    loading: false, tableData: [] as any[], treeData: [] as any[],
    treeProps: { children: 'children', label: 'name' }, queryParams: { mineId: null as number | null },
    changeType: '', dateRange: null as any
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
            { sensorId: 'CH4-004', sensorName: '甲烷传感器4', changeType: 1, oldValue: '--', newValue: '0.15%', operator: '张三', changeTime: '2026-03-09 10:00:00', reason: '新增测点' },
            { sensorId: 'CH4-002', sensorName: '甲烷传感器2', changeType: 3, oldValue: '0.5%', newValue: '0.8%', operator: '李四', changeTime: '2026-03-08 14:30:00', reason: '调校传感器' },
            { sensorId: 'CO-001', sensorName: '一氧化碳传感器', changeType: 4, oldValue: '正常', newValue: '检修中', operator: '王五', changeTime: '2026-03-07 09:00:00', reason: '定期检修' },
            { sensorId: 'TEMP-002', sensorName: '温度传感器2', changeType: 2, oldValue: '28℃', newValue: '--', operator: '赵六', changeTime: '2026-03-06 16:00:00', reason: '设备报废' },
        ];
        if (state.changeType) data = data.filter((x: any) => x.changeType === parseInt(state.changeType));
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
