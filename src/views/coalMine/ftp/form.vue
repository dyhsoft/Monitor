<template>
    <el-dialog v-model="state.visible" :title="state.title" width="600px" destroy-on-close>
        <el-form :model="state.form" ref="formRef" :rules="rules" label-width="100px">
            <el-form-item label="所属煤矿" prop="mineId">
                <el-select v-model="state.form.mineId" placeholder="请选择煤矿" filterable :disabled="!!state.mineId">
                    <el-option v-for="item in mineList" :key="item.id" :label="item.name" :value="item.id" />
                </el-select>
            </el-form-item>
            <el-form-item label="FTP地址" prop="host">
                <el-input v-model="state.form.host" placeholder="请输入FTP服务器地址" />
            </el-form-item>
            <el-form-item label="端口" prop="port">
                <el-input-number v-model="state.form.port" :min="1" :max="65535" :step="1" :stepStrictly="true" />
            </el-form-item>
            <el-form-item label="FTP用户名" prop="username">
                <el-input v-model="state.form.username" placeholder="请输入FTP用户名" />
            </el-form-item>
            <el-form-item label="密码" prop="password">
                <el-input v-model="state.form.password" type="password" placeholder="请输入密码" show-password />
            </el-form-item>
            <el-form-item label="根目录" prop="rootDirectory">
                <el-input v-model="state.form.rootDirectory" placeholder="请输入根目录" />
            </el-form-item>
            <el-form-item label="绑定系统" prop="bindSystem">
                <el-select v-model="state.form.bindSystem" placeholder="请选择绑定系统">
                    <el-option label="安全监测" value="安全监测" />
                    <el-option label="人员定位" value="人员定位" />
                    <el-option label="矿压监测" value="矿压监测" />
                    <el-option label="水文监测" value="水文监测" />
                </el-select>
            </el-form-item>
            <el-form-item label="允许IP">
                <el-input v-model="state.form.allowedIp" placeholder="请输入允许IP，多个用逗号分隔" />
            </el-form-item>
            <el-form-item label="是否启用">
                <el-switch v-model="state.enabled" :active-value="1" :inactive-value="2" />
            </el-form-item>
        </el-form>
        <template #footer>
            <el-button @click="state.visible = false">取 消</el-button>
            <el-button type="primary" @click="submit">确 定</el-button>
        </template>
    </el-dialog>
</template>

<script lang="ts" setup>
import { ref, reactive, computed, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, FtpConfigApi } from '/@/api-services/api';

const props = defineProps<{
    mineList?: any[]
}>();

const emit = defineEmits(['refresh']);
const formRef = ref();
const state = reactive({
    visible: false,
    title: '新增FTP配置',
    form: {} as any,
    id: 0,
    mineId: 0 as number
});

const enabled = computed({
    get: () => state.form.enabled || 1,
    set: (val) => state.form.enabled = val
});

const rules = {
    mineId: [{ required: true, message: '请选择煤矿', trigger: 'change' }],
    host: [{ required: true, message: '请输入FTP服务器地址', trigger: 'blur' }],
    port: [{ required: true, message: '请输入端口', trigger: 'blur' }],
    username: [{ required: true, message: '请输入FTP用户名', trigger: 'blur' }],
    password: [{ required: true, message: '请输入密码', trigger: 'blur' }],
    rootDirectory: [{ required: true, message: '请输入根目录', trigger: 'blur' }],
    bindSystem: [{ required: true, message: '请选择绑定系统', trigger: 'change' }]
};

function open(row?: any, mineId?: number) {
    state.visible = true;
    state.form = { enabled: 1 };
    state.id = 0;
    state.mineId = mineId || 0;
    if (row) {
        state.title = '编辑FTP配置';
        state.id = row.id;
        getAPI(FtpConfigApi).get(row.id).then((res) => {
            state.form = { ...res.data.result };
        });
    } else {
        state.title = '新增FTP配置';
        if (mineId) {
            state.form.mineId = mineId;
        }
    }
}

function submit() {
    formRef.value?.validate(async (valid) => {
        if (valid) {
            if (state.id) {
                await getAPI(FtpConfigApi).update({ ...state.form, id: state.id });
                ElMessage.success('更新成功');
            } else {
                await getAPI(FtpConfigApi).add(state.form);
                ElMessage.success('新增成功');
            }
            state.visible = false;
            emit('refresh');
        }
    });
}

defineExpose({ open });
</script>
