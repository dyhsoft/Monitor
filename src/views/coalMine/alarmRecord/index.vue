<template>
    <div class="alarm-record-container">
        <el-card shadow="hover">
            <el-form :inline="true">
                <el-form-item label="煤矿">
                    <el-select v-model="state.queryParams.mineId" placeholder="请选择煤矿" clearable filterable @change="handleQuery">
                        <el-option v-for="item in state.mineList" :key="item.id" :label="item.name" :value="item.id" />
                    </el-select>
                </el-form-item>
                <el-form-item label="报警类型">
                    <el-select v-model="state.queryParams.alarmType" placeholder="请选择" clearable @change="handleQuery">
                        <el-option label="超员报警" value="超员报警" />
                        <el-option label="超时报警" value="超时报警" />
                        <el-option label="区域报警" value="区域报警" />
                        <el-option label="低电量报警" value="低电量报警" />
                        <el-option label="传感器报警" value="传感器报警" />
                        <el-option label="水害报警" value="水害报警" />
                        <el-option label="矿压报警" value="矿压报警" />
                    </el-select>
                </el-form-item>
                <el-form-item label="状态">
                    <el-select v-model="state.queryParams.status" placeholder="请选择" clearable @change="handleQuery">
                        <el-option label="未处理" :value="1" />
                        <el-option label="已处理" :value="2" />
                    </el-select>
                </el-form-item>
                <el-form-item label="时间范围">
                    <el-date-picker
                        v-model="state.dateRange"
                        type="daterange"
                        range-separator="至"
                        start-placeholder="开始日期"
                        end-placeholder="结束日期"
                        @change="handleQuery"
                    />
                </el-form-item>
                <el-form-item>
                    <el-button type="primary" icon="ele-Search" @click="handleQuery"> 查询 </el-button>
                    <el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
                </el-form-item>
            </el-form>
        </el-card>

        <el-card class="full-table" shadow="hover" style="margin-top: 10px">
            <el-table :data="state.tableData" v-loading="state.loading" border stripe>
                <el-table-column type="index" label="序号" width="60" align="center" />
                <el-table-column prop="mineName" label="所属煤矿" min-width="120" align="center" />
                <el-table-column prop="sensorCode" label="传感器编号" min-width="120" align="center" />
                <el-table-column prop="alarmType" label="报警类型" min-width="100" align="center" />
                <el-table-column prop="alarmValue" label="报警值" min-width="80" align="center" />
                <el-table-column prop="threshold" label="阈值" min-width="80" align="center" />
                <el-table-column prop="alarmTime" label="报警时间" width="160" align="center" />
                <el-table-column prop="status" label="状态" width="80" align="center">
                    <template #default="scope">
                        <el-tag :type="scope.row.status === 1 ? 'danger' : 'success'">
                            {{ scope.row.status === 1 ? '未处理' : '已处理' }}
                        </el-tag>
                    </template>
                </el-table-column>
                <el-table-column label="操作" width="100" align="center" fixed="right">
                    <template #default="scope">
                        <el-button icon="ele-Check" text type="success" @click="handleAlarm(scope.row)" v-if="scope.row.status === 1"> 处理 </el-button>
                    </template>
                </el-table-column>
            </el-table>
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
import { ElMessage } from 'element-plus';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, AlarmRecordApi } from '/@/api-services/api';

const state = reactive({
    loading: false,
    total: 0,
    mineList: [] as any[],
    tableData: [] as any[],
    dateRange: [] as any[],
    queryParams: {
        page: 1,
        pageSize: 10,
        mineId: null as number | null,
        alarmType: '',
        status: null as number | null
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

async function handleQuery() {
    state.loading = true;
    try {
        const params = {
            page: state.queryParams.page,
            pageSize: state.queryParams.pageSize,
            mineId: state.queryParams.mineId,
            alarmType: state.queryParams.alarmType || undefined,
            status: state.queryParams.status,
            startTime: state.dateRange?.[0] || undefined,
            endTime: state.dateRange?.[1] || undefined
        };
        const res = await getAPI(AlarmRecordApi).getPage(params);
        state.tableData = res.data.result?.rows || res.data.result || [];
        state.total = res.data.result?.total || 0;
    } catch (error) {
        console.error('加载报警记录失败:', error);
        state.tableData = [];
        state.total = 0;
    } finally {
        state.loading = false;
    }
}

function resetQuery() {
    state.queryParams.mineId = null;
    state.queryParams.alarmType = '';
    state.queryParams.status = null;
    state.dateRange = [];
    handleQuery();
}

function handleAlarm(row: any) {
    ElMessage.info('处理报警功能开发中');
}
</script>

<style scoped>
.alarm-record-container { padding: 10px; }
.full-table { height: calc(100vh - 220px); overflow: auto; }
</style>
