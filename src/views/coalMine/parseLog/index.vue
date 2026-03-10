<template>
    <div class="parse-log-container">
        <el-card shadow="hover">
            <el-form :model="state.queryParams" :inline="true">
                <el-form-item label="煤矿">
                    <el-select v-model="state.queryParams.mineId" placeholder="请选择煤矿" clearable filterable @change="handleQuery">
                        <el-option v-for="item in state.mineList" :key="item.id" :label="item.name" :value="item.id" />
                    </el-select>
                </el-form-item>
                <el-form-item label="数据类型">
                    <el-select v-model="state.queryParams.dataType" placeholder="请选择" clearable @change="handleQuery">
                        <el-option label="CDSS(传感器)" value="CDSS" />
                        <el-option label="RWSS(人员定位)" value="RWSS" />
                        <el-option label="KYGL(矿压)" value="KYGL" />
                        <el-option label="SWJC(水文)" value="SWJC" />
                    </el-select>
                </el-form-item>
                <el-form-item label="状态">
                    <el-select v-model="state.queryParams.status" placeholder="请选择" clearable @change="handleQuery">
                        <el-option label="成功" :value="1" />
                        <el-option label="失败" :value="0" />
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
                <el-table-column prop="fileName" label="文件名" min-width="200" align="center" show-overflow-tooltip />
                <el-table-column prop="dataType" label="数据类型" width="100" align="center" />
                <el-table-column prop="bindSystem" label="绑定系统" width="100" align="center" />
                <el-table-column prop="recordCount" label="记录数" width="80" align="center" />
                <el-table-column prop="duration" label="耗时(ms)" width="80" align="center" />
                <el-table-column label="状态" width="80" align="center">
                    <template #default="scope">
                        <el-tag :type="scope.row.status === 1 ? 'success' : 'danger'">
                            {{ scope.row.status === 1 ? '成功' : '失败' }}
                        </el-tag>
                    </template>
                </el-table-column>
                <el-table-column prop="errorMessage" label="错误信息" min-width="150" align="center" show-overflow-tooltip />
                <el-table-column prop="createTime" label="解析时间" width="160" align="center" />
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
import { CoalMineApi, ParseLogApi } from '/@/api-services/api';

const state = reactive({
    loading: false,
    total: 0,
    tableData: [] as any[],
    mineList: [] as any[],
    queryParams: {
        page: 1,
        pageSize: 10,
        mineId: null as number | null,
        dataType: '',
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

function handleQuery() {
    state.loading = true;
    getAPI(ParseLogApi).getPage(state.queryParams).then((res) => {
        state.tableData = res.data.result?.items || [];
        state.total = res.data.result?.total || 0;
    }).finally(() => {
        state.loading = false;
    });
}

function resetQuery() {
    state.queryParams.mineId = null;
    state.queryParams.dataType = '';
    state.queryParams.status = null;
    handleQuery();
}
</script>

<style scoped>
.parse-log-container { padding: 10px; }
.full-table { height: calc(100vh - 220px); overflow: auto; }
</style>
