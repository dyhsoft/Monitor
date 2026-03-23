<template>
    <div class="location-layout">
        <div class="left-tree">
            <el-card shadow="hover">
                <template #header>
                    <span style="font-weight: bold;">选择煤矿</span>
                </template>
                <el-tree 
                    :data="state.treeData" 
                    :props="state.treeProps"
                    @node-click="handleNodeClick"
                    node-key="id"
                    default-expand-all
                    highlight-current
                />
            </el-card>
        </div>
        <div class="right-content">
            <el-row :gutter="10">
                <el-col :span="6">
                    <el-card shadow="hover">
                        <template #header>井下实时人数</template>
                        <div class="stat-number">{{ state.stats.inMineCount }}</div>
                    </el-card>
                </el-col>
                <el-col :span="6">
                    <el-card shadow="hover">
                        <template #header>超时人数</template>
                        <div class="stat-number warning">{{ state.stats.overtimeCount }}</div>
                    </el-card>
                </el-col>
                <el-col :span="6">
                    <el-card shadow="hover">
                        <template #header>超员人数</template>
                        <div class="stat-number danger">{{ state.stats.overCount }}</div>
                    </el-card>
                </el-col>
                <el-col :span="6">
                    <el-card shadow="hover">
                        <template #header>基站报警</template>
                        <div class="stat-number danger">{{ state.stats.alarmCount }}</div>
                    </el-card>
                </el-col>
            </el-row>

            <el-row :gutter="10" style="margin-top: 10px">
                <el-col :span="12">
                    <el-card shadow="hover">
                        <template #header>井下人员实时列表</template>
                        <el-table :data="state.personList" height="350" border>
                            <el-table-column type="index" label="序号" width="60" align="center" />
                            <el-table-column prop="personName" label="姓名" align="center" />
                            <el-table-column prop="cardId" label="卡号" align="center" />
                            <el-table-column prop="deptName" label="部门" align="center" />
                            <el-table-column prop="areaName" label="位置" align="center" />
                        </el-table>
                    </el-card>
                </el-col>
                <el-col :span="12">
                    <el-card shadow="hover">
                        <template #header>实时报警信息</template>
                        <el-table :data="state.alarmList" height="350" border>
                            <el-table-column type="index" label="序号" width="60" align="center" />
                            <el-table-column prop="alarmType" label="类型" width="80" align="center">
                                <template #default="scope">
                                    <el-tag :type="getAlarmTypeTag(scope.row.alarmType)">{{ getAlarmTypeText(scope.row.alarmType) }}</el-tag>
                                </template>
                            </el-table-column>
                            <el-table-column prop="personName" label="姓名" align="center" />
                            <el-table-column prop="areaName" label="区域" align="center" />
                            <el-table-column prop="alarmMessage" label="报警内容" show-overflow-tooltip />
                        </el-table>
                    </el-card>
                </el-col>
            </el-row>
        </div>
    </div>
</template>

<script lang="ts" setup>
import { onMounted, reactive } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, PersonApi, LocationAlarmApi } from '/@/api-services/api';

const state = reactive({
    treeData: [] as any[],
    treeProps: {
        children: 'children',
        label: 'name'
    },
    queryParams: {
        mineId: null as number | null
    },
    stats: { inMineCount: 0, overtimeCount: 0, overCount: 0, alarmCount: 0 },
    personList: [] as any[],
    alarmList: [] as any[]
});

onMounted(() => {
    loadMineTree();
});

function loadMineTree() {
    getAPI(CoalMineApi).getList({ page: 1, pageSize: 1000 }).then((res) => {
        state.treeData = (res.data.result || []).map((item: any) => ({
            id: item.id,
            name: item.name,
            children: []
        }));
    });
}

function handleNodeClick(data: any) {
    state.queryParams.mineId = data.id;
    loadData();
}

function loadData() {
    if (!state.queryParams.mineId) return;
    
    getAPI(PersonApi).getRealTime(state.queryParams.mineId).then((res) => {
        state.personList = res.data.result || [];
        state.stats.inMineCount = state.personList.length;
    });

    getAPI(LocationAlarmApi).getRealTime(state.queryParams.mineId).then((res) => {
        state.alarmList = res.data.result || [];
        state.stats.alarmCount = state.alarmList.length;
        state.stats.overtimeCount = state.alarmList.filter((x: any) => x.alarmType === 1).length;
        state.stats.overCount = state.alarmList.filter((x: any) => x.alarmType === 2).length;
    });
}

function getAlarmTypeTag(type: number) {
    const tags = ['', 'danger', 'warning', 'danger'];
    return tags[type] || 'info';
}

function getAlarmTypeText(type: number) {
    const texts = ['', '超时', '超员', '基站报警'];
    return texts[type] || '未知';
}
</script>

<style scoped>
.location-layout {
    display: flex;
    gap: 10px;
    height: calc(100vh - 150px);
}
.left-tree {
    width: 250px;
    overflow: auto;
}
.right-content {
    flex: 1;
    overflow: auto;
}
.stat-number { font-size: 32px; font-weight: bold; text-align: center; color: #409eff; }
.stat-number.warning { color: #e6a23c; }
.stat-number.danger { color: #f56c6c; }
</style>
