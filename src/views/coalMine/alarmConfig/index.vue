<template>
    <div class="alarm-config-container">
        <el-tabs v-model="state.activeTab" @tab-change="handleTabChange">
            <!-- Tab1: 报警配置列表 -->
            <el-tab-pane label="报警配置" name="config">
                <el-card shadow="hover">
                    <el-form :inline="true">
                        <el-form-item label="煤矿">
                            <el-select v-model="state.queryParams.mineId" placeholder="请选择煤矿" clearable filterable @change="handleQuery">
                                <el-option v-for="item in state.mineList" :key="item.id" :label="item.name" :value="item.id" />
                            </el-select>
                        </el-form-item>
                        <el-form-item label="报警类别">
                            <el-select v-model="state.queryParams.alarmCategory" placeholder="请选择" clearable @change="handleQuery">
                                <el-option v-for="item in alarmCategories" :key="item.value" :label="item.label" :value="item.value" />
                            </el-select>
                        </el-form-item>
                        <el-form-item label="报警级别">
                            <el-select v-model="state.queryParams.alarmLevel" placeholder="请选择" clearable @change="handleQuery">
                                <el-option label="一般" :value="1" />
                                <el-option label="重要" :value="2" />
                                <el-option label="紧急" :value="3" />
                                <el-option label="预警" :value="4" />
                            </el-select>
                        </el-form-item>
                        <el-form-item>
                            <el-button type="primary" icon="ele-Search" @click="handleQuery"> 查询 </el-button>
                            <el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
                        </el-form-item>
                        <el-form-item>
                            <el-button type="primary" icon="ele-Plus" @click="openAdd"> 新增 </el-button>
                        </el-form-item>
                    </el-form>
                </el-card>
                <el-card class="full-table" shadow="hover" style="margin-top: 10px">
                    <el-table :data="state.tableData" v-loading="state.loading" border stripe>
                        <el-table-column type="index" label="序号" width="60" align="center" />
                        <el-table-column prop="mineName" label="所属煤矿" min-width="120" align="center" show-overflow-tooltip />
                        <el-table-column prop="alarmCategory" label="报警类别" min-width="100" align="center" />
                        <el-table-column prop="alarmName" label="报警名称" min-width="180" align="center" show-overflow-tooltip />
                        <el-table-column prop="alarmLevelName" label="级别" width="70" align="center">
                            <template #default="scope">
                                <el-tag :type="getLevelType(scope.row.alarmLevel)">{{ scope.row.alarmLevelName }}</el-tag>
                            </template>
                        </el-table-column>
                        <el-table-column prop="description" label="描述" min-width="200" align="center" show-overflow-tooltip />
                        <el-table-column prop="timeThreshold" label="时间阈值" width="80" align="center">
                            <template #default="scope">
                                {{ scope.row.timeThreshold ? `${scope.row.timeThreshold}秒` : '-' }}
                            </template>
                        </el-table-column>
                        <el-table-column prop="enabledName" label="状态" width="70" align="center">
                            <template #default="scope">
                                <el-switch v-model="scope.row.enabled" :active-value="1" :inactive-value="0" @change="handleSwitchChange(scope.row)" />
                            </template>
                        </el-table-column>
                        <el-table-column label="操作" width="150" align="center" fixed="right">
                            <template #default="scope">
                                <el-button icon="ele-Edit" text type="primary" @click="openEdit(scope.row)"> 编辑 </el-button>
                                <el-button icon="ele-Delete" text type="danger" @click="delData(scope.row)"> 删除 </el-button>
                            </template>
                        </el-table-column>
                    </el-table>
                    <el-pagination 
                        v-model:current-page="state.queryParams.page" 
                        v-model:page-size="state.queryParams.pageSize"
                        :page-sizes="[10, 20, 50, 100]"
                        :total="state.total"
                        layout="total, sizes, prev, pager, next, jumper"
                        @size-change="handleQuery"
                        @current-change="handleQuery"
                        style="margin-top: 10px" 
                    />
                </el-card>
            </el-tab-pane>

            <!-- Tab2: 预定义模板 -->
            <el-tab-pane label="报警模板" name="preset">
                <el-card shadow="hover">
                    <el-alert title="点击下方预定义模板可快速添加报警配置" type="info" :closable="false" style="margin-bottom: 15px" />
                    <el-form :inline="true" style="margin-bottom: 10px">
                        <el-form-item label="报警类别">
                            <el-select v-model="state.presetCategory" placeholder="请选择" clearable @change="loadPresets">
                                <el-option v-for="item in alarmCategories" :key="item.value" :label="item.label" :value="item.value" />
                            </el-select>
                        </el-form-item>
                        <el-form-item>
                            <el-button type="primary" icon="ele-Refresh" @click="loadPresets"> 刷新 </el-button>
                        </el-form-item>
                    </el-form>
                    <el-table :data="state.presetList" v-loading="state.presetLoading" border stripe max-height="500">
                        <el-table-column type="index" label="序号" width="60" align="center" />
                        <el-table-column prop="alarmCategory" label="报警类别" width="100" align="center" />
                        <el-table-column prop="alarmName" label="报警名称" min-width="180" align="center" show-overflow-tooltip />
                        <el-table-column prop="description" label="描述" min-width="250" align="center" show-overflow-tooltip />
                        <el-table-column prop="alarmLevel" label="级别" width="70" align="center">
                            <template #default="scope">
                                <el-tag :type="getLevelType(scope.row.alarmLevel)">{{ getLevelName(scope.row.alarmLevel) }}</el-tag>
                            </template>
                        </el-table-column>
                        <el-table-column label="操作" width="100" align="center" fixed="right">
                            <template #default="scope">
                                <el-button type="primary" size="small" @click="usePreset(scope.row)"> 添加 </el-button>
                            </template>
                        </el-table-column>
                    </el-table>
                </el-card>
            </el-tab-pane>
        </el-tabs>

        <!-- 新增/编辑对话框 -->
        <el-dialog v-model="state.dialogVisible" :title="state.isEdit ? '编辑报警配置' : '新增报警配置'" width="600px">
            <el-form :model="state.form" :rules="state.rules" ref="formRef" label-width="100px">
                <el-form-item label="煤矿" prop="mineId">
                    <el-select v-model="state.form.mineId" placeholder="请选择煤矿" filterable>
                        <el-option v-for="item in state.mineList" :key="item.id" :label="item.name" :value="item.id" />
                    </el-select>
                </el-form-item>
                <el-form-item label="报警名称" prop="alarmName">
                    <el-input v-model="state.form.alarmName" placeholder="请输入报警名称" />
                </el-form-item>
                <el-form-item label="报警编码" prop="alarmCode">
                    <el-input v-model="state.form.alarmCode" placeholder="请输入报警编码" />
                </el-form-item>
                <el-form-item label="报警类别" prop="alarmCategory">
                    <el-select v-model="state.form.alarmCategory" placeholder="请选择报警类别">
                        <el-option v-for="item in alarmCategories" :key="item.value" :label="item.label" :value="item.value" />
                    </el-select>
                </el-form-item>
                <el-form-item label="报警级别" prop="alarmLevel">
                    <el-radio-group v-model="state.form.alarmLevel">
                        <el-radio :value="1">一般</el-radio>
                        <el-radio :value="2">重要</el-radio>
                        <el-radio :value="3">紧急</el-radio>
                        <el-radio :value="4">预警</el-radio>
                    </el-radio-group>
                </el-form-item>
                <el-form-item label="描述说明">
                    <el-input v-model="state.form.description" type="textarea" :rows="2" />
                </el-form-item>
                <el-form-item label="时间阈值(秒)">
                    <el-input-number v-model="state.form.timeThreshold" :min="0" :step="10" />
                </el-form-item>
                <el-form-item label="通知人员">
                    <el-select v-model="state.form.notifyUserIds" multiple placeholder="请选择通知人员" filterable>
                        <el-option v-for="item in state.userList" :key="item.id" :label="item.name" :value="item.id" />
                    </el-select>
                </el-form-item>
                <el-form-item label="是否启用">
                    <el-switch v-model="state.form.enabled" :active-value="1" :inactive-value="0" />
                </el-form-item>
                <el-form-item label="备注">
                    <el-input v-model="state.form.remark" type="textarea" :rows="2" />
                </el-form-item>
            </el-form>
            <template #footer>
                <el-button @click="state.dialogVisible = false">取消</el-button>
                <el-button type="primary" @click="submitForm" :loading="state.submitLoading">确定</el-button>
            </template>
        </el-dialog>
    </div>
</template>

<script lang="ts" setup>
import { onMounted, reactive, ref } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, AlarmConfigApi } from '/@/api-services/api';

const formRef = ref();

const alarmCategories = [
    { label: '传感器报警', value: '传感器报警' },
    { label: '设备监控', value: '设备监控' },
    { label: '数据监控', value: '数据监控' },
    { label: '人员行为', value: '人员行为' },
    { label: '智能预测', value: '智能预测' }
];

const state = reactive({
    activeTab: 'config',
    loading: false,
    total: 0,
    mineList: [] as any[],
    userList: [] as any[],
    tableData: [] as any[],
    presetLoading: false,
    presetCategory: '',
    presetList: [] as any[],
    queryParams: {
        page: 1,
        pageSize: 10,
        mineId: null as number | null,
        alarmCategory: '',
        alarmLevel: null as number | null
    },
    dialogVisible: false,
    isEdit: false,
    submitLoading: false,
    form: {} as any,
    rules: {
        mineId: [{ required: true, message: '请选择煤矿', trigger: 'change' }],
        alarmName: [{ required: true, message: '请输入报警名称', trigger: 'blur' }],
        alarmCode: [{ required: true, message: '请输入报警编码', trigger: 'blur' }],
        alarmCategory: [{ required: true, message: '请选择报警类别', trigger: 'change' }],
        alarmLevel: [{ required: true, message: '请选择报警级别', trigger: 'change' }]
    }
});

onMounted(() => {
    loadMineList();
    loadUserList();
    handleQuery();
    loadPresets();
});

function loadMineList() {
    getAPI(CoalMineApi).getList({ page: 1, pageSize: 1000 }).then((res) => {
        state.mineList = res.data.result || [];
    });
}

function loadUserList() {
    // 暂时使用空列表，后续可以调用用户API获取
    state.userList = [];
}

function handleQuery() {
    state.loading = true;
    getAPI(AlarmConfigApi).getPage({
        ...state.queryParams,
        mineId: state.queryParams.mineId || undefined,
        alarmCategory: state.queryParams.alarmCategory || undefined,
        alarmLevel: state.queryParams.alarmLevel || undefined
    }).then((res) => {
        state.tableData = res.data.result?.items || [];
        state.total = res.data.result?.total || 0;
    }).finally(() => {
        state.loading = false;
    });
}

function resetQuery() {
    state.queryParams.mineId = null;
    state.queryParams.alarmCategory = '';
    state.queryParams.alarmLevel = null;
    handleQuery();
}

function handleTabChange(tabName: string) {
    if (tabName === 'preset') {
        loadPresets();
    }
}

function loadPresets() {
    state.presetLoading = true;
    getAPI(AlarmConfigApi).getPresetAlarmConfigs().then((res) => {
        let list = res.data || [];
        if (state.presetCategory) {
            list = list.filter((item: any) => item.alarmCategory === state.presetCategory);
        }
        state.presetList = list;
    }).finally(() => {
        state.presetLoading = false;
    });
}

function getLevelType(level: number): string {
    return level === 1 ? 'info' : level === 2 ? 'warning' : level === 3 ? 'danger' : 'success';
}

function getLevelName(level: number): string {
    return level === 1 ? '一般' : level === 2 ? '重要' : level === 3 ? '紧急' : '预警';
}

function openAdd() {
    state.isEdit = false;
    state.form = {
        mineId: state.queryParams.mineId || (state.mineList[0]?.id || null),
        alarmName: '',
        alarmCode: '',
        alarmCategory: '传感器报警',
        alarmLevel: 1,
        description: '',
        timeThreshold: 0,
        notifyUserIds: [],
        enabled: 1,
        remark: ''
    };
    state.dialogVisible = true;
}

function openEdit(row: any) {
    state.isEdit = true;
    state.form = { 
        ...row,
        notifyUserIds: row.notifyUserIds ? JSON.parse(row.notifyUserIds) : []
    };
    state.dialogVisible = true;
}

async function submitForm() {
    const valid = await formRef.value?.validate();
    if (!valid) return;

    state.submitLoading = true;
    try {
        const submitData = {
            ...state.form,
            notifyUserIds: state.form.notifyUserIds?.length > 0 ? JSON.stringify(state.form.notifyUserIds) : ''
        };
        
        if (state.isEdit) {
            await getAPI(AlarmConfigApi).update(submitData);
            ElMessage.success('编辑成功');
        } else {
            await getAPI(AlarmConfigApi).add(submitData);
            ElMessage.success('新增成功');
        }
        state.dialogVisible = false;
        handleQuery();
    } catch (error: any) {
        ElMessage.error(error.message || '操作失败');
    } finally {
        state.submitLoading = false;
    }
}

function delData(row: any) {
    ElMessageBox.confirm('确定删除该报警配置吗？', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
    }).then(async () => {
        await getAPI(AlarmConfigApi).delete(row.id);
        ElMessage.success('删除成功');
        handleQuery();
    });
}

function handleSwitchChange(row: any) {
    getAPI(AlarmConfigApi).setEnabled(row.id, row.enabled).then(() => {
        ElMessage.success(row.enabled === 1 ? '已启用' : '已禁用');
    }).catch(() => {
        row.enabled = row.enabled === 1 ? 0 : 1;
    });
}

function usePreset(row: any) {
    if (!state.queryParams.mineId) {
        ElMessage.warning('请先选择煤矿');
        return;
    }
    
    ElMessageBox.confirm(`确定添加报警配置"${row.alarmName}"吗？`, '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'info'
    }).then(async () => {
        await getAPI(AlarmConfigApi).add({
            mineId: state.queryParams.mineId,
            alarmName: row.alarmName,
            alarmCode: row.alarmCode,
            alarmCategory: row.alarmCategory,
            alarmLevel: row.alarmLevel,
            description: row.description,
            condition: row.condition,
            timeThreshold: row.timeThreshold,
            enabled: 1
        });
        ElMessage.success('添加成功');
        handleQuery();
        state.activeTab = 'config';
    });
}
</script>

<style scoped>
.alarm-config-container { padding: 10px; }
.full-table { height: calc(100vh - 280px); overflow: auto; }
</style>
