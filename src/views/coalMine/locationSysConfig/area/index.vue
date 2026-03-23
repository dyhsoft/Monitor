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
                    <el-form-item><el-button type="primary" icon="ele-Plus" @click="openAdd">新增区域</el-button></el-form-item>
                </el-form>
            </el-card>
            <el-card shadow="hover" style="margin-top: 10px">
                <el-table :data="state.tableData" v-loading="state.loading" border stripe>
                    <el-table-column type="index" label="序号" width="60" align="center" />
                    <el-table-column prop="areaName" label="区域名称" align="center" />
                    <el-table-column prop="areaType" label="区域类型" align="center">
                        <template #default="scope">
                            <el-tag>{{ ['采煤面', '掘进面', '巷道', '重要区域'][scope.row.areaType - 1] }}</el-tag>
                        </template>
                    </el-table-column>
                    <el-table-column prop="limitCount" label="限定人数" align="center" />
                    <el-table-column prop="isKey" label="重点区域" align="center">
                        <template #default="scope">
                            <el-tag :type="scope.row.isKey ? 'danger' : 'info'">{{ scope.row.isKey ? '是' : '否' }}</el-tag>
                        </template>
                    </el-table-column>
                    <el-table-column prop="enabled" label="状态" align="center">
                        <template #default="scope">
                            <el-switch v-model="scope.row.enabled" />
                        </template>
                    </el-table-column>
                    <el-table-column label="操作" width="120" align="center">
                        <template #default="scope">
                            <el-button icon="ele-Delete" text type="danger" @click="delData(scope.row)">删除</el-button>
                        </template>
                    </el-table-column>
                </el-table>
            </el-card>
        </div>
        <el-dialog v-model="state.visible" title="新增区域" width="500px">
            <el-form :model="state.form" label-width="100px">
                <el-form-item label="区域名称"><el-input v-model="state.form.areaName" /></el-form-item>
                <el-form-item label="区域类型">
                    <el-select v-model="state.form.areaType">
                        <el-option label="采煤面" :value="1" />
                        <el-option label="掘进面" :value="2" />
                        <el-option label="巷道" :value="3" />
                        <el-option label="重要区域" :value="4" />
                    </el-select>
                </el-form-item>
                <el-form-item label="限定人数"><el-input-number v-model="state.form.limitCount" :min="1" /></el-form-item>
                <el-form-item label="重点区域"><el-switch v-model="state.form.isKey" /></el-form-item>
            </el-form>
            <template #footer>
                <el-button @click="state.visible = false">取消</el-button>
                <el-button type="primary" @click="submit">确定</el-button>
            </template>
        </el-dialog>
    </div>
</template>

<script setup lang="ts">
import { onMounted, reactive } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, PersonApi } from '/@/api-services/api';
import { ElMessage } from 'element-plus';

const state = reactive({
    loading: false, tableData: [] as any[], treeData: [] as any[],
    treeProps: { children: 'children', label: 'name' }, queryParams: { mineId: null as number | null },
    visible: false, form: {} as any
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
        const res = await getAPI(PersonApi).getAreaStatistics(state.queryParams.mineId);
        const data = res.data.result || [];
        state.tableData = data.map((item: any) => ({
            areaName: item.areaName,
            areaType: 3,
            limitCount: 20,
            isKey: false,
            enabled: true
        }));
    } catch (error) {
        console.error('加载区域数据失败:', error);
        state.tableData = [];
    } finally {
        state.loading = false;
    }
}

function openAdd() {
    if (!state.queryParams.mineId) { ElMessage.warning('请先选择煤矿'); return; }
    state.form = { mineId: state.queryParams.mineId, limitCount: 10, isKey: false, enabled: true };
    state.visible = true;
}

function submit() {
    ElMessage.success('保存成功');
    state.visible = false;
    loadData();
}

function delData(row: any) {
    state.tableData = state.tableData.filter((x: any) => x.areaName !== row.areaName);
    ElMessage.success('删除成功');
}
</script>

<style scoped>
.page-layout { display: flex; gap: 10px; height: calc(100vh - 150px); }
.left-tree { width: 250px; overflow: auto; }
.right-content { flex: 1; overflow: auto; }
</style>
