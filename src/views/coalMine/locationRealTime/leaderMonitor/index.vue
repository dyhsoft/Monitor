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
import { CoalMineApi } from '/@/api-services/api';

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
            { leaderName: '张三', position: '矿长', cardId: 'C001', currentArea: '采煤面A', inTime: '2026-03-09 08:30', duration: 8.5, status: 1 },
            { leaderName: '李四', position: '副矿长', cardId: 'C002', currentArea: '掘进面1', inTime: '2026-03-09 09:00', duration: 7.8, status: 1 },
            { leaderName: '王五', position: '总工程师', cardId: 'C003', currentArea: '主井', inTime: '', duration: 0, status: 0 },
        ];
        state.loading = false;
    }, 300);
}
</script>

<style scoped>
.page-layout { display: flex; gap: 10px; height: calc(100vh - 150px); }
.left-tree { width: 250px; overflow: auto; }
.right-content { flex: 1; overflow: auto; }
</style>
