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
                    <el-form-item><el-button type="primary" icon="ele-Plus" @click="openAdd">新增设备</el-button></el-form-item>
                </el-form>
            </el-card>
            <el-card shadow="hover" style="margin-top: 10px">
                <el-table :data="state.tableData" v-loading="state.loading" border stripe>
                    <el-table-column type="index" label="序号" width="60" align="center" />
                    <el-table-column prop="deviceName" label="设备名称" align="center" />
                    <el-table-column prop="deviceId" label="设备编号" align="center" />
                    <el-table-column prop="deviceType" label="设备类型" align="center">
                        <template #default="scope">
                            <el-tag>{{ ['定位基站', '读卡器', '标识卡'][scope.row.deviceType - 1] }}</el-tag>
                        </template>
                    </el-table-column>
                    <el-table-column prop="areaName" label="安装区域" align="center" />
                    <el-table-column prop="status" label="状态" align="center">
                        <template #default="scope">
                            <el-tag :type="scope.row.status === 1 ? 'success' : 'danger'">
                                {{ scope.row.status === 1 ? '在线' : '离线' }}
                            </el-tag>
                        </template>
                    </el-table-column>
                    <el-table-column label="操作" width="150" align="center">
                        <template #default="scope">
                            <el-button icon="ele-Edit" text @click="editData(scope.row)">编辑</el-button>
                            <el-button icon="ele-Delete" text type="danger" @click="delData(scope.row)">删除</el-button>
                        </template>
                    </el-table-column>
                </el-table>
            </el-card>
        </div>
    </div>
</template>

<script setup lang="ts">
import { onMounted, reactive } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi } from '/@/api-services/api';
import { ElMessage } from 'element-plus';

const state = reactive({
    loading: false, tableData: [] as any[], treeData: [] as any[],
    treeProps: { children: 'children', label: 'name' }, queryParams: { mineId: null as number | null }
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
        state.tableData = [
            { deviceName: '基站ST001', deviceId: 'ST001', deviceType: 1, areaName: '主井', status: 1 },
            { deviceName: '基站ST002', deviceId: 'ST002', deviceType: 1, areaName: '副井', status: 1 },
            { deviceName: '读卡器R001', deviceId: 'R001', deviceType: 2, areaName: '主井', status: 1 },
        ];
        state.loading = false;
    }, 300);
}

function openAdd() {
    if (!state.queryParams.mineId) { ElMessage.warning('请先选择煤矿'); return; }
    ElMessage.info('新增设备功能');
}

function editData(row: any) {
    ElMessage.info('编辑: ' + row.deviceName);
}

function delData(row: any) {
    state.tableData = state.tableData.filter((x: any) => x.deviceId !== row.deviceId);
    ElMessage.success('删除成功');
}
</script>

<style scoped>
.page-layout { display: flex; gap: 10px; height: calc(100vh - 150px); }
.left-tree { width: 250px; overflow: auto; }
.right-content { flex: 1; overflow: auto; }
</style>
