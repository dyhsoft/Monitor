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
                    <el-form-item label="传感器">
                        <el-select v-model="state.sensorId" placeholder="请选择传感器" clearable style="width: 150px;">
                            <el-option label="压力传感器01" value="1" />
                            <el-option label="位移传感器01" value="2" />
                            <el-option label="锚杆应力01" value="3" />
                        </el-select>
                    </el-form-item>
                    <el-form-item label="时间范围">
                        <el-date-picker v-model="state.dateRange" type="datetimerange" range-separator="至" start-placeholder="开始" end-placeholder="结束" value-format="YYYY-MM-DD HH:mm:ss" />
                    </el-form-item>
                    <el-form-item><el-button type="primary" @click="loadData">查询</el-button></el-form-item>
                </el-form>
            </el-card>
            <el-card shadow="hover" style="margin-top: 10px">
                <div ref="chartRef" style="width: 100%; height: 350px;"></div>
            </el-card>
            <el-row :gutter="10" style="margin-top: 10px">
                <el-col :span="8">
                    <el-card shadow="hover">
                        <div class="stat-value">最大值: {{ state.stats.max }} MPa</div>
                    </el-card>
                </el-col>
                <el-col :span="8">
                    <el-card shadow="hover">
                        <div class="stat-value">最小值: {{ state.stats.min }} MPa</div>
                    </el-card>
                </el-col>
                <el-col :span="8">
                    <el-card shadow="hover">
                        <div class="stat-value">平均值: {{ state.stats.avg }} MPa</div>
                    </el-card>
                </el-col>
            </el-row>
        </div>
    </div>
</template>

<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, PressureApi } from '/@/api-services/api';

const chartRef = ref<HTMLElement>();

const state = reactive({
    loading: false,
    treeData: [] as any[],
    treeProps: { children: 'children', label: 'name' },
    queryParams: { mineId: null as number | null, sensorId: '', dateRange: null as any },
    stats: { max: 0, min: 0, avg: 0 },
    tableData: [] as any[]
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
        const params: any = { mineId: state.queryParams.mineId, page: 1, pageSize: 100 };
        if (state.queryParams.dateRange && state.queryParams.dateRange.length === 2) {
            params.startTime = state.queryParams.dateRange[0];
            params.endTime = state.queryParams.dateRange[1];
        }
        const res = await getAPI(PressureApi).getHistoryPage(params);
        const data = res.data.result?.rows || res.data.result || [];
        state.tableData = data;
        if (data.length > 0) {
            const values = data.map((d: any) => Number(d.value) || 0);
            state.stats.max = Math.max(...values).toFixed(2);
            state.stats.min = Math.min(...values).toFixed(2);
            state.stats.avg = (values.reduce((a: number, b: number) => a + b, 0) / values.length).toFixed(2);
        }
    } catch (error) {
        console.error('加载矿压曲线数据失败:', error);
    } finally {
        state.loading = false;
    }
}
</script>

<style scoped>
.page-layout { display: flex; gap: 10px; height: calc(100vh - 150px); }
.left-tree { width: 250px; overflow: auto; }
.right-content { flex: 1; overflow: auto; }
.stat-value { text-align: center; font-size: 16px; font-weight: bold; padding: 10px; }
</style>
