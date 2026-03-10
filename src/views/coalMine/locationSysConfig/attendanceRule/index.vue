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
                <el-form :model="state.form" label-width="120px">
                    <el-form-item label="上班时间">
                        <el-time-picker v-model="state.form.workStartTime" placeholder="选择时间" style="width: 200px;" />
                    </el-form-item>
                    <el-form-item label="下班时间">
                        <el-time-picker v-model="state.form.workEndTime" placeholder="选择时间" style="width: 200px;" />
                    </el-form-item>
                    <el-form-item label="迟到阈值(分钟)">
                        <el-input-number v-model="state.form.lateThreshold" :min="0" />
                    </el-form-item>
                    <el-form-item label="早退阈值(分钟)">
                        <el-input-number v-model="state.form.earlyLeaveThreshold" :min="0" />
                    </el-form-item>
                    <el-form-item label="最大工作时长(小时)">
                        <el-input-number v-model="state.form.maxWorkHours" :min="1" :max="24" />
                    </el-form-item>
                    <el-form-item>
                        <el-button type="primary" @click="save">保存配置</el-button>
                    </el-form-item>
                </el-form>
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
    treeData: [] as any[], treeProps: { children: 'children', label: 'name' },
    queryParams: { mineId: null as number | null },
    form: { workStartTime: new Date(2026, 0, 1, 8, 0), workEndTime: new Date(2026, 0, 1, 17, 0), lateThreshold: 15, earlyLeaveThreshold: 15, maxWorkHours: 12 }
});

onMounted(() => { loadMineTree(); });

function loadMineTree() {
    getAPI(CoalMineApi).getList({ page: 1, pageSize: 1000 }).then((res) => {
        state.treeData = (res.data.result || []).map((item: any) => ({ id: item.id, name: item.name, children: [] }));
    });
}

function handleNodeClick(data: any) {
    state.queryParams.mineId = data.id;
}

function save() {
    if (!state.queryParams.mineId) { ElMessage.warning('请先选择煤矿'); return; }
    ElMessage.success('保存成功');
}
</script>

<style scoped>
.page-layout { display: flex; gap: 10px; height: calc(100vh - 150px); }
.left-tree { width: 250px; overflow: auto; }
.right-content { flex: 1; overflow: auto; }
</style>
