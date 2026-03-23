<template>
    <div class="location-history-container">
        <el-card shadow="hover">
            <el-form :inline="true">
                <el-form-item label="煤矿">
                    <el-select v-model="state.queryParams.mineId" placeholder="请选择煤矿" clearable @change="handleQuery">
                        <el-option v-for="item in state.mineList" :key="item.id" :label="item.name" :value="item.id" />
                    </el-select>
                </el-form-item>
                <el-form-item label="姓名">
                    <el-input v-model="state.queryParams.personName" placeholder="请输入姓名" clearable @keyup.enter="handleQuery" />
                </el-form-item>
                <el-form-item label="日期">
                    <el-date-picker v-model="state.dateRange" type="daterange" range-separator="至" start-placeholder="开始日期" end-placeholder="结束日期" value-format="YYYY-MM-DD" @change="handleQuery" />
                </el-form-item>
                <el-form-item>
                    <el-button type="primary" icon="ele-Search" @click="handleQuery">查询</el-button>
                    <el-button icon="ele-Refresh" @click="resetQuery">重置</el-button>
                </el-form-item>
            </el-form>
        </el-card>

        <el-card style="margin-top: 10px" shadow="hover">
            <el-tabs v-model="state.activeTab">
                <el-tab-pane label="人员下井记录" name="records">
                    <el-table :data="state.tableData" v-loading="state.loading" border stripe height="500">
                        <el-table-column type="index" label="序号" width="60" align="center" />
                        <el-table-column prop="mineName" label="煤矿" width="100" align="center" />
                        <el-table-column prop="personName" label="姓名" width="80" align="center" />
                        <el-table-column prop="cardId" label="卡号" width="100" align="center" />
                        <el-table-column prop="deptName" label="部门" width="100" align="center" />
                        <el-table-column prop="areaName" label="区域" width="120" align="center" />
                        <el-table-column prop="status" label="状态" width="80" align="center">
                            <template #default="scope">
                                <el-tag :type="scope.row.status === 1 ? 'success' : 'info'">{{ scope.row.status === 1 ? '井下' : '井上' }}</el-tag>
                            </template>
                        </el-table-column>
                        <el-table-column prop="updateTime" label="时间" width="160" align="center" />
                    </el-table>
                </el-tab-pane>
                
                <el-tab-pane label="报警记录" name="alarms">
                    <el-table :data="state.alarmTableData" v-loading="state.alarmLoading" border stripe height="500">
                        <el-table-column type="index" label="序号" width="60" align="center" />
                        <el-table-column prop="mineName" label="煤矿" width="100" align="center" />
                        <el-table-column prop="alarmType" label="类型" width="80" align="center">
                            <template #default="scope">
                                <el-tag :type="getAlarmTypeTag(scope.row.alarmType)">{{ getAlarmTypeText(scope.row.alarmType) }}</el-tag>
                            </template>
                        </el-table-column>
                        <el-table-column prop="personName" label="姓名" width="80" align="center" />
                        <el-table-column prop="areaName" label="区域" width="120" align="center" />
                        <el-table-column prop="alarmMessage" label="报警内容" show-overflow-tooltip />
                        <el-table-column prop="status" label="状态" width="80" align="center">
                            <template #default="scope">
                                <el-tag :type="scope.row.status === 1 ? 'success' : 'warning'">{{ scope.row.status === 1 ? '已处理' : '未处理' }}</el-tag>
                            </template>
                        </el-table-column>
                        <el-table-column prop="alarmTime" label="时间" width="160" align="center" />
                    </el-table>
                </el-tab-pane>
            </el-tabs>
            <el-pagination 
                v-model:current-page="state.queryParams.page" 
                v-model:page-size="state.queryParams.pageSize"
                :page-sizes="[10, 20, 50, 100]"
                :total="state.total"
                layout="total, sizes, prev, pager, next, jumper"
                @size-change="handleQuery"
                @current-change="handleQuery"
                style="margin-top: 10px" 
            />
        </el-card>
    </div>
</template>

<script lang="ts" setup>
import { onMounted, reactive } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, PersonApi, LocationAlarmApi } from '/@/api-services/api';

const state = reactive({
    activeTab: 'records',
    loading: false,
    alarmLoading: false,
    total: 0,
    tableData: [] as any[],
    alarmTableData: [] as any[],
    mineList: [] as any[],
    dateRange: null as any,
    queryParams: {
        page: 1,
        pageSize: 10,
        mineId: null as number | null,
        personName: ''
    }
});

onMounted(() => {
    loadMineList();
    handleQuery();
});

function loadMineList() {
    getAPI(CoalMineApi).getList({ page: 1, pageSize: 1000 }).then((res) => {
        state.mineList = res.data.result || [];
    });
}

function handleQuery() {
    if (state.activeTab === 'records') {
        state.loading = true;
        const params = { ...state.queryParams };
        if (state.dateRange && state.dateRange.length === 2) {
            params.startTime = state.dateRange[0];
            params.endTime = state.dateRange[1];
        }
        getAPI(PersonApi).getRecordPage(params).then((res) => {
            state.tableData = res.data.result?.items || [];
            state.total = res.data.result?.total || 0;
        }).finally(() => {
            state.loading = false;
        });
    } else {
        state.alarmLoading = true;
        const params = { ...state.queryParams };
        if (state.dateRange && state.dateRange.length === 2) {
            params.startTime = state.dateRange[0];
            params.endTime = state.dateRange[1];
        }
        getAPI(LocationAlarmApi).getPage(params).then((res) => {
            state.alarmTableData = res.data.result?.items || [];
            state.total = res.data.result?.total || 0;
        }).finally(() => {
            state.alarmLoading = false;
        });
    }
}

function resetQuery() {
    state.queryParams.mineId = null;
    state.queryParams.personName = '';
    state.dateRange = null;
    handleQuery();
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
.location-history-container { padding: 10px; }
</style>
