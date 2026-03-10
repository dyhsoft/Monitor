<template>
    <div class="water-data-container">
        <el-card shadow="hover">
            <el-form :model="state.queryParams" :inline="true">
                <el-form-item label="煤矿">
                    <el-select v-model="state.queryParams.mineId" placeholder="请选择煤矿" clearable filterable @change="handleQuery">
                        <el-option v-for="item in state.mineList" :key="item.id" :label="item.name" :value="item.id" />
                    </el-select>
                </el-form-item>
                <el-form-item label="传感器类型">
                    <el-select v-model="state.queryParams.sensorType" placeholder="请选择" clearable @change="handleQuery">
                        <el-option label="水位" value="水位" />
                        <el-option label="流量" value="流量" />
                        <el-option label="排水量" value="排水量" />
                    </el-select>
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
                <el-table-column prop="mineName" label="煤矿" min-width="100" align="center" />
                <el-table-column prop="sensorCode" label="传感器编号" min-width="140" align="center" />
                <el-table-column prop="sensorName" label="传感器名称" min-width="150" align="center" />
                <el-table-column prop="sensorType" label="类型" width="100" align="center" />
                <el-table-column prop="value" label="监测值" width="100" align="center" />
                <el-table-column prop="unit" label="单位" width="80" align="center" />
                <el-table-column prop="updateTime" label="更新时间" width="160" align="center" />
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
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, WaterDataApi } from '/@/api-services/api';

const state = reactive({
    loading: false,
    total: 0,
    tableData: [] as any[],
    mineList: [] as any[],
    queryParams: {
        page: 1,
        pageSize: 10,
        mineId: null as number | null,
        sensorType: ''
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
    state.loading = true;
    getAPI(WaterDataApi).getPage(state.queryParams).then((res) => {
        state.tableData = res.data.result?.items || [];
        state.total = res.data.result?.total || 0;
    }).finally(() => {
        state.loading = false;
    });
}

function resetQuery() {
    state.queryParams.mineId = null;
    state.queryParams.sensorType = '';
    handleQuery();
}
</script>

<style scoped>
.water-data-container { padding: 10px; }
.full-table { height: calc(100vh - 220px); overflow: auto; }
</style>
