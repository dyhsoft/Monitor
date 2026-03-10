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
                <el-table :data="state.tableData" v-loading="state.loading" border stripe>
                    <el-table-column type="index" label="序号" width="60" align="center" />
                    <el-table-column prop="sensorType" label="传感器类型" align="center" />
                    <el-table-column prop="alarmThreshold" label="报警阈值" align="center" />
                    <el-table-column prop="dangerThreshold" label="危险阈值" align="center" />
                    <el-table-column prop="unit" label="单位" align="center" />
                    <el-table-column label="操作" width="100" align="center">
                        <template #default="scope">
                            <el-button icon="ele-Edit" text @click="editData(scope.row)">编辑</el-button>
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

onMounted(() => { loadMineTree(); loadData(); });

function loadMineTree() {
    getAPI(CoalMineApi).getList({ page: 1, pageSize: 1000 }).then((res) => {
        state.treeData = (res.data.result || []).map((item: any) => ({ id: item.id, name: item.name, children: [] }));
    });
}

function handleNodeClick(data: any) { state.queryParams.mineId = data.id; }

function loadData() {
    state.tableData = [
        { sensorType: '甲烷(CH4)', alarmThreshold: 0.5, dangerThreshold: 1.0, unit: '%' },
        { sensorType: '一氧化碳(CO)', alarmThreshold: 24, dangerThreshold: 50, unit: 'ppm' },
        { sensorType: '温度', alarmThreshold: 30, dangerThreshold: 40, unit: '℃' },
        { sensorType: '风速', alarmThreshold: 8, dangerThreshold: 10, unit: 'm/s' },
    ];
}

function editData(row: any) { ElMessage.info('编辑: ' + row.sensorType); }
</script>

<style scoped>
.page-layout { display: flex; gap: 10px; height: calc(100vh - 150px); }
.left-tree { width: 250px; overflow: auto; }
.right-content { flex: 1; overflow: auto; }
</style>
