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
                    <el-form-item><el-button type="primary" icon="ele-Plus" @click="openAdd">新增矿领导</el-button></el-form-item>
                </el-form>
                <el-table :data="state.tableData" v-loading="state.loading" border stripe>
                    <el-table-column type="index" label="序号" width="60" align="center" />
                    <el-table-column prop="personName" label="姓名" align="center" />
                    <el-table-column prop="cardId" label="卡号" align="center" />
                    <el-table-column prop="deptName" label="部门" align="center" />
                    <el-table-column prop="position" label="职务" align="center" />
                    <el-table-column label="状态" width="80" align="center">
                        <template #default="scope">
                            <el-tag :type="scope.row.enabled === 1 ? 'success' : 'danger'">{{ scope.row.enabled === 1 ? '启用' : '禁用' }}</el-tag>
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

        <el-dialog v-model="state.visible" title="新增矿领导" width="500px">
            <el-form :model="state.form" :rules="rules" ref="formRef" label-width="80px">
                <el-form-item label="姓名" prop="personName"><el-input v-model="state.form.personName" /></el-form-item>
                <el-form-item label="卡号" prop="cardId"><el-input v-model="state.form.cardId" /></el-form-item>
                <el-form-item label="部门"><el-input v-model="state.form.deptName" /></el-form-item>
                <el-form-item label="职务"><el-input v-model="state.form.position" /></el-form-item>
            </el-form>
            <template #footer>
                <el-button @click="state.visible = false">取消</el-button>
                <el-button type="primary" @click="submit">确定</el-button>
            </template>
        </el-dialog>
    </div>
</template>

<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, LocationLeaderConfigApi } from '/@/api-services/api';

const formRef = ref();
const state = reactive({
    loading: false, tableData: [] as any[], treeData: [] as any[],
    treeProps: { children: 'children', label: 'name' },
    queryParams: { mineId: null as number | null },
    visible: false, form: {} as any,
    rules: { personName: [{ required: true }], cardId: [{ required: true }] }
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
    getAPI(LocationLeaderConfigApi).getPage({ mineId: state.queryParams.mineId, page: 1, pageSize: 100 }).then((res) => {
        state.tableData = res.data.result?.items || [];
    }).finally(() => { state.loading = false; });
}

function openAdd() {
    if (!state.queryParams.mineId) { ElMessage.warning('请先选择煤矿'); return; }
    state.form = { enabled: 1, mineId: state.queryParams.mineId };
    state.visible = true;
}

function submit() {
    getAPI(LocationLeaderConfigApi).add(state.form).then(() => {
        ElMessage.success('新增成功');
        state.visible = false;
        loadData();
    });
}

function delData(row: any) {
    ElMessageBox.confirm('确定删除?').then(() => {
        getAPI(LocationLeaderConfigApi).delete(row.id).then(() => { ElMessage.success('删除成功'); loadData(); });
    });
}
</script>

<style scoped>
.page-layout { display: flex; gap: 10px; height: calc(100vh - 150px); }
.left-tree { width: 250px; overflow: auto; }
.right-content { flex: 1; overflow: auto; }
</style>
