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
                <el-table :data="state.tableData" v-loading="state.loading" border stripe height="450">
                    <el-table-column type="index" label="序号" width="60" align="center" />
                    <el-table-column prop="leaderName" label="领导姓名" align="center" />
                    <el-table-column prop="position" label="职务" align="center" />
                    <el-table-column prop="cardId" label="标识卡号" align="center" />
                    <el-table-column prop="currentArea" label="当前位置" align="center" />
                    <el-table-column prop="inTime" label="入井时间" width="160" align="center" />
                    <el-table-column prop="duration" label="时长(小时)" align="center" />
                    <el-table-column prop="status" label="状态" align="center">
                        <template #default="scope">
                            <el-tag :type="scope.row.status === 1 ? 'success' : 'info'">
                                {{ scope.row.status === 1 ? '井下' : '井上' }}
                            </el-tag>
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
        const res = await getAPI(PersonApi).getRealtimePage({ mineId: state.queryParams.mineId, page: 1, pageSize: 50 });
        const data = res.data.result?.rows || res.data.result || [];
        state.tableData = data.slice(0, 10).map((item: any) => ({
            leaderName: item.personName,
            position: '员工',
            cardId: item.cardId,
            currentArea: item.areaName || '未知',
            inTime: item.updateTime,
            duration: 1.5,
            status: 1
        }));
    } catch (error) {
        console.error('加载领导监控数据失败:', error);
        state.tableData = [];
    } finally {
        state.loading = false;
    }
}
</script>

<style scoped>
.page-layout { display: flex; gap: 10px; height: calc(100vh - 150px); }
.left-tree { width: 250px; overflow: auto; }
.right-content { flex: 1; overflow: auto; }
</style>
