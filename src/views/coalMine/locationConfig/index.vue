<template>
    <div class="location-config-container">
        <el-tabs v-model="state.activeTab">
            <el-tab-pane label="矿领导配置" name="leader">
                <el-card shadow="hover">
                    <el-form :inline="true">
                        <el-form-item label="煤矿">
                            <el-select v-model="state.leaderQuery.mineId" placeholder="请选择煤矿" clearable @change="loadLeader">
                                <el-option v-for="item in state.mineList" :key="item.id" :label="item.name" :value="item.id" />
                            </el-select>
                        </el-form-item>
                        <el-form-item>
                            <el-button type="primary" icon="ele-Plus" @click="openLeaderAdd">新增</el-button>
                        </el-form-item>
                    </el-form>
                    <el-table :data="state.leaderList" v-loading="state.leaderLoading" border stripe>
                        <el-table-column type="index" label="序号" width="60" align="center" />
                        <el-table-column prop="mineName" label="煤矿" width="120" align="center" />
                        <el-table-column prop="personName" label="姓名" width="100" align="center" />
                        <el-table-column prop="cardId" label="卡号" width="120" align="center" />
                        <el-table-column prop="deptName" label="部门" width="120" align="center" />
                        <el-table-column prop="position" label="职务" width="100" align="center" />
                        <el-table-column label="状态" width="80" align="center">
                            <template #default="scope">
                                <el-tag :type="scope.row.enabled === 1 ? 'success' : 'danger'">{{ scope.row.enabled === 1 ? '启用' : '禁用' }}</el-tag>
                            </template>
                        </el-table-column>
                        <el-table-column label="操作" width="120" align="center">
                            <template #default="scope">
                                <el-button icon="ele-Delete" text type="danger" @click="delLeader(scope.row)">删除</el-button>
                            </template>
                        </el-table-column>
                    </el-table>
                </el-card>
            </el-tab-pane>

            <el-tab-pane label="限定人数配置" name="limit">
                <el-card shadow="hover">
                    <el-form :inline="true">
                        <el-form-item label="煤矿">
                            <el-select v-model="state.limitQuery.mineId" placeholder="请选择煤矿" clearable @change="loadLimit">
                                <el-option v-for="item in state.mineList" :key="item.id" :label="item.name" :value="item.id" />
                            </el-select>
                        </el-form-item>
                        <el-form-item>
                            <el-button type="primary" icon="ele-Plus" @click="openLimitAdd">新增</el-button>
                        </el-form-item>
                    </el-form>
                    <el-table :data="state.limitList" v-loading="state.limitLoading" border stripe>
                        <el-table-column type="index" label="序号" width="60" align="center" />
                        <el-table-column prop="mineName" label="煤矿" width="120" align="center" />
                        <el-table-column prop="areaName" label="区域" width="150" align="center" />
                        <el-table-column prop="limitCount" label="限定人数" width="100" align="center" />
                        <el-table-column label="状态" width="80" align="center">
                            <template #default="scope">
                                <el-tag :type="scope.row.enabled === 1 ? 'success' : 'danger'">{{ scope.row.enabled === 1 ? '启用' : '禁用' }}</el-tag>
                            </template>
                        </el-table-column>
                        <el-table-column label="操作" width="120" align="center">
                            <template #default="scope">
                                <el-button icon="ele-Delete" text type="danger" @click="delLimit(scope.row)">删除</el-button>
                            </template>
                        </el-table-column>
                    </el-table>
                </el-card>
            </el-tab-pane>
        </el-tabs>

        <!-- 矿领导新增对话框 -->
        <el-dialog v-model="state.leaderVisible" title="新增矿领导" width="500px">
            <el-form :model="state.leaderForm" :rules="state.leaderRules" ref="leaderFormRef" label-width="80px">
                <el-form-item label="煤矿" prop="mineId">
                    <el-select v-model="state.leaderForm.mineId" placeholder="请选择煤矿">
                        <el-option v-for="item in state.mineList" :key="item.id" :label="item.name" :value="item.id" />
                    </el-select>
                </el-form-item>
                <el-form-item label="姓名" prop="personName">
                    <el-input v-model="state.leaderForm.personName" />
                </el-form-item>
                <el-form-item label="卡号" prop="cardId">
                    <el-input v-model="state.leaderForm.cardId" />
                </el-form-item>
                <el-form-item label="部门">
                    <el-input v-model="state.leaderForm.deptName" />
                </el-form-item>
                <el-form-item label="职务">
                    <el-input v-model="state.leaderForm.position" />
                </el-form-item>
            </el-form>
            <template #footer>
                <el-button @click="state.leaderVisible = false">取消</el-button>
                <el-button type="primary" @click="submitLeader">确定</el-button>
            </template>
        </el-dialog>

        <!-- 限定人数新增对话框 -->
        <el-dialog v-model="state.limitVisible" title="新增限定人数" width="500px">
            <el-form :model="state.limitForm" :rules="state.limitRules" ref="limitFormRef" label-width="80px">
                <el-form-item label="煤矿" prop="mineId">
                    <el-select v-model="state.limitForm.mineId" placeholder="请选择煤矿">
                        <el-option v-for="item in state.mineList" :key="item.id" :label="item.name" :value="item.id" />
                    </el-select>
                </el-form-item>
                <el-form-item label="区域">
                    <el-input v-model="state.limitForm.areaName" placeholder="不填表示整个矿井" />
                </el-form-item>
                <el-form-item label="限定人数" prop="limitCount">
                    <el-input-number v-model="state.limitForm.limitCount" :min="1" />
                </el-form-item>
            </el-form>
            <template #footer>
                <el-button @click="state.limitVisible = false">取消</el-button>
                <el-button type="primary" @click="submitLimit">确定</el-button>
            </template>
        </el-dialog>
    </div>
</template>

<script lang="ts" setup>
import { onMounted, reactive, ref } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, LocationLeaderConfigApi, LocationLimitConfigApi } from '/@/api-services/api';

const leaderFormRef = ref();
const limitFormRef = ref();

const state = reactive({
    activeTab: 'leader',
    mineList: [] as any[],
    leaderQuery: { mineId: null as number | null },
    leaderLoading: false,
    leaderList: [] as any[],
    leaderVisible: false,
    leaderForm: {} as any,
    leaderRules: {
        mineId: [{ required: true, message: '请选择煤矿', trigger: 'change' }],
        personName: [{ required: true, message: '请输入姓名', trigger: 'blur' }],
        cardId: [{ required: true, message: '请输入卡号', trigger: 'blur' }]
    },
    limitQuery: { mineId: null as number | null },
    limitLoading: false,
    limitList: [] as any[],
    limitVisible: false,
    limitForm: {} as any,
    limitRules: {
        mineId: [{ required: true, message: '请选择煤矿', trigger: 'change' }],
        limitCount: [{ required: true, message: '请输入限定人数', trigger: 'blur' }]
    }
});

onMounted(() => {
    loadMineList();
    loadLeader();
    loadLimit();
});

function loadMineList() {
    getAPI(CoalMineApi).getList({ page: 1, pageSize: 1000 }).then((res) => {
        state.mineList = res.data.result || [];
    });
}

function loadLeader() {
    state.leaderLoading = true;
    getAPI(LocationLeaderConfigApi).getPage(state.leaderQuery).then((res) => {
        state.leaderList = res.data.result?.items || [];
    }).finally(() => {
        state.leaderLoading = false;
    });
}

function loadLimit() {
    state.limitLoading = true;
    getAPI(LocationLimitConfigApi).getPage(state.limitQuery).then((res) => {
        state.limitList = res.data.result?.items || [];
    }).finally(() => {
        state.limitLoading = false;
    });
}

function openLeaderAdd() {
    state.leaderForm = { enabled: 1 };
    state.leaderVisible = true;
}

function submitLeader() {
    leaderFormRef.value?.validate(async (valid) => {
        if (valid) {
            await getAPI(LocationLeaderConfigApi).add(state.leaderForm);
            ElMessage.success('新增成功');
            state.leaderVisible = false;
            loadLeader();
        }
    });
}

function delLeader(row: any) {
    ElMessageBox.confirm('确定删除该配置吗？', '提示', { type: 'warning' }).then(() => {
        getAPI(LocationLeaderConfigApi).delete(row.id).then(() => {
            ElMessage.success('删除成功');
            loadLeader();
        });
    });
}

function openLimitAdd() {
    state.limitForm = { enabled: 1, limitCount: 100 };
    state.limitVisible = true;
}

function submitLimit() {
    limitFormRef.value?.validate(async (valid) => {
        if (valid) {
            await getAPI(LocationLimitConfigApi).add(state.limitForm);
            ElMessage.success('新增成功');
            state.limitVisible = false;
            loadLimit();
        }
    });
}

function delLimit(row: any) {
    ElMessageBox.confirm('确定删除该配置吗？', '提示', { type: 'warning' }).then(() => {
        getAPI(LocationLimitConfigApi).delete(row.id).then(() => {
            ElMessage.success('删除成功');
            loadLimit();
        });
    });
}
</script>

<style scoped>
.location-config-container { padding: 10px; }
</style>
