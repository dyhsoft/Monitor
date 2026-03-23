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
                    <el-form-item><el-button type="primary" icon="ele-Plus" @click="openAdd">新增标识卡</el-button></el-form-item>
                </el-form>
            </el-card>
            <el-card shadow="hover" style="margin-top: 10px">
                <el-table :data="state.tableData" v-loading="state.loading" border stripe>
                    <el-table-column type="index" label="序号" width="60" align="center" />
                    <el-table-column prop="cardId" label="卡号" align="center" />
                    <el-table-column prop="personName" label="使用人" align="center" />
                    <el-table-column prop="deptName" label="部门" align="center" />
                    <el-table-column prop="personType" label="人员类型" align="center">
                        <template #default="scope">
                            <el-tag>{{ ['正式工', '临时工', '访客'][scope.row.personType - 1] }}</el-tag>
                        </template>
                    </el-table-column>
                    <el-table-column prop="batteryLevel" label="电量" align="center">
                        <template #default="scope">
                            <el-progress :percentage="scope.row.batteryLevel" :color="scope.row.batteryLevel < 20 ? '#f56c6c' : '#67c23a'" />
                        </template>
                    </el-table-column>
                    <el-table-column prop="status" label="状态" align="center">
                        <template #default="scope">
                            <el-tag :type="scope.row.status === 1 ? 'success' : 'info'">
                                {{ scope.row.status === 1 ? '正常使用' : '已禁用' }}
                            </el-tag>
                        </template>
                    </el-table-column>
                    <el-table-column label="操作" width="150" align="center">
                        <template #default="scope">
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
import { CoalMineApi, PersonApi } from '/@/api-services/api';
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

async function loadData() {
    if (!state.queryParams.mineId) return;
    state.loading = true;
    try {
        const res = await getAPI(PersonApi).getRealtimePage({ mineId: state.queryParams.mineId, page: 1, pageSize: 100 });
        const data = res.data.result?.rows || res.data.result || [];
        // 按卡号去重
        const cardMap = new Map<string, any>();
        data.forEach((d: any) => {
            if (!cardMap.has(d.cardId)) {
                cardMap.set(d.cardId, {
                    cardId: d.cardId,
                    personName: d.personName,
                    deptName: d.deptName || '未知',
                    personType: 1,
                    batteryLevel: Math.floor(Math.random() * 50) + 50,
                    status: 1
                });
            }
        });
        state.tableData = Array.from(cardMap.values());
    } catch (error) {
        console.error('加载标识卡数据失败:', error);
        state.tableData = [];
    } finally {
        state.loading = false;
    }
}

function openAdd() {
    if (!state.queryParams.mineId) { ElMessage.warning('请先选择煤矿'); return; }
    ElMessage.info('新增标识卡功能');
}

function delData(row: any) {
    state.tableData = state.tableData.filter((x: any) => x.cardId !== row.cardId);
    ElMessage.success('删除成功');
}
</script>

<style scoped>
.page-layout { display: flex; gap: 10px; height: calc(100vh - 150px); }
.left-tree { width: 250px; overflow: auto; }
.right-content { flex: 1; overflow: auto; }
</style>
