<template>
    <div class="person-location-container">
        <el-card shadow="hover">
            <el-form :model="state.queryParams" :inline="true">
                <el-form-item label="煤矿">
                    <el-select v-model="state.queryParams.mineId" placeholder="请选择煤矿" clearable filterable @change="handleQuery">
                        <el-option v-for="item in state.mineList" :key="item.id" :label="item.name" :value="item.id" />
                    </el-select>
                </el-form-item>
                <el-form-item label="姓名">
                    <el-input v-model="state.queryParams.personName" placeholder="请输入姓名" clearable @keyup.enter="handleQuery" />
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
                <el-table-column prop="cardId" label="卡号" min-width="100" align="center" />
                <el-table-column prop="personName" label="姓名" min-width="80" align="center" />
                <el-table-column prop="deptName" label="部门" min-width="100" align="center" />
                <el-table-column prop="stationId" label="分站" min-width="80" align="center" />
                <el-table-column prop="areaName" label="区域" min-width="100" align="center" />
                <el-table-column prop="x" label="坐标X" width="120" align="center" />
                <el-table-column prop="y" label="坐标Y" width="120" align="center" />
                <el-table-column prop="z" label="深度" width="80" align="center" />
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
import { CoalMineApi, PersonLocationApi } from '/@/api-services/api';

const state = reactive({
    loading: false,
    total: 0,
    tableData: [] as any[],
    mineList: [] as any[],
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
    state.loading = true;
    getAPI(PersonLocationApi).getPage(state.queryParams).then((res) => {
        state.tableData = res.data.result?.items || [];
        state.total = res.data.result?.total || 0;
    }).finally(() => {
        state.loading = false;
    });
}

function resetQuery() {
    state.queryParams.mineId = null;
    state.queryParams.personName = '';
    handleQuery();
}
</script>

<style scoped>
.person-location-container { padding: 10px; }
.full-table { height: calc(100vh - 220px); overflow: auto; }
</style>
