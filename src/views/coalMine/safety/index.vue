<template>
    <div class="safety-layout">
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
            <el-card shadow="hover">
                <el-form :inline="true">
                    <el-form-item label="传感器类型">
                        <el-select v-model="state.queryParams.sensorType" placeholder="请选择" clearable @change="handleQuery">
                            <el-option label="甲烷(CH4)" value="A01" />
                            <el-option label="一氧化碳(CO)" value="A02" />
                            <el-option label="氧气(O2)" value="A10" />
                            <el-option label="温度" value="A13" />
                            <el-option label="湿度" value="A14" />
                            <el-option label="风速" value="A15" />
                            <el-option label="水位" value="A20" />
                            <el-option label="流量" value="A21" />
                        </el-select>
                    </el-form-item>
                    <el-form-item>
                        <el-button type="primary" icon="ele-Search" @click="handleQuery">查询</el-button>
                    </el-form-item>
                </el-form>
            </el-card>

            <el-card shadow="hover" style="margin-top: 10px">
                <el-table :data="state.tableData" v-loading="state.loading" border stripe height="500">
                    <el-table-column type="index" label="序号" width="60" align="center" />
                    <el-table-column prop="sensorCode" label="传感器编号" min-width="140" align="center" />
                    <el-table-column prop="sensorName" label="传感器名称" min-width="150" align="center" />
                    <el-table-column prop="sensorType" label="类型" width="80" align="center" />
                    <el-table-column prop="value" label="监测值" width="100" align="center" />
                    <el-table-column prop="unit" label="单位" width="60" align="center" />
                    <el-table-column label="状态" width="80" align="center">
                        <template #default="scope">
                            <el-tag :type="getStatusType(scope.row.status)">{{ getStatusText(scope.row.status) }}</el-tag>
                        </template>
                    </el-table-column>
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
    </div>
</template>

<script lang="ts" setup>
import { onMounted, reactive } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, SafetyDataApi } from '/@/api-services/api';

const state = reactive({
    loading: false,
    total: 0,
    tableData: [] as any[],
    treeData: [] as any[],
    treeProps: {
        children: 'children',
        label: 'name'
    },
    queryParams: {
        page: 1,
        pageSize: 10,
        mineId: null as number | null,
        sensorType: ''
    }
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
    handleQuery();
}

function handleQuery() {
    if (!state.queryParams.mineId) {
        state.tableData = [];
        return;
    }
    state.loading = true;
    getAPI(SafetyDataApi).getPage(state.queryParams).then((res) => {
        state.tableData = res.data.result?.items || [];
        state.total = res.data.result?.total || 0;
    }).finally(() => {
        state.loading = false;
    });
}

function getStatusType(status: number) {
    const types = ['', 'danger', 'warning', 'info', 'success'];
    return types[status] || 'info';
}

function getStatusText(status: number) {
    const texts = ['', '报警', '断电', '故障', '离线', '正常'];
    return texts[status] || '正常';
}
</script>

<style scoped>
.safety-layout {
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
</style>
