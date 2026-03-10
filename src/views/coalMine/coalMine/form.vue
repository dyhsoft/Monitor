<template>
    <el-dialog v-model="state.visible" :title="state.title" width="600px" destroy-on-close>
        <el-form :model="state.form" ref="formRef" :rules="rules" label-width="100px">
            <el-form-item label="煤矿编码" prop="code">
                <el-input v-model="state.form.code" placeholder="请输入煤矿编码" />
            </el-form-item>
            <el-form-item label="煤矿名称" prop="name">
                <el-input v-model="state.form.name" placeholder="请输入煤矿名称" />
            </el-form-item>
            <el-form-item label="地址">
                <el-input v-model="state.form.address" placeholder="请输入地址" />
            </el-form-item>
            <el-form-item label="FTP根目录">
                <el-input v-model="state.form.ftpPath" placeholder="请输入FTP根目录" />
            </el-form-item>
            <el-form-item label="数据类型">
                <el-select v-model="state.form.dataType" placeholder="请选择数据类型">
                    <el-option label="JSON" value="json" />
                    <el-option label="CSV" value="csv" />
                    <el-option label="自定义" value="custom" />
                </el-select>
            </el-form-item>
            <el-form-item label="是否启用">
                <el-switch v-model="state.enabled" :active-value="1" :inactive-value="2" />
            </el-form-item>
            <el-form-item label="备注">
                <el-input v-model="state.form.remark" type="textarea" :rows="3" placeholder="请输入备注" />
            </el-form-item>
        </el-form>
        <template #footer>
            <el-button @click="state.visible = false">取 消</el-button>
            <el-button type="primary" @click="submit">确 定</el-button>
        </template>
    </el-dialog>
</template>

<script lang="ts" setup>
import { ref, reactive, computed } from 'vue';
import { ElMessage } from 'element-plus';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi } from '/@/api-services/api';

const emit = defineEmits(['refresh']);
const formRef = ref();
const state = reactive({
    visible: false,
    title: '新增煤矿',
    form: {} as any,
    id: 0
});

const enabled = computed({
    get: () => state.form.enabled || 1,
    set: (val) => state.form.enabled = val
});

const rules = {
    code: [{ required: true, message: '请输入煤矿编码', trigger: 'blur' }],
    name: [{ required: true, message: '请输入煤矿名称', trigger: 'blur' }]
};

function open(row?: any) {
    state.visible = true;
    state.form = {};
    state.id = 0;
    if (row) {
        state.title = '编辑煤矿';
        state.id = row.id;
        getAPI(CoalMineApi).get(row.id).then((res) => {
            state.form = { ...res.data.result };
        });
    } else {
        state.title = '新增煤矿';
        state.form.enabled = 1;
    }
}

function submit() {
    formRef.value?.validate(async (valid) => {
        if (valid) {
            if (state.id) {
                await getAPI(CoalMineApi).update({ ...state.form, id: state.id });
                ElMessage.success('更新成功');
            } else {
                await getAPI(CoalMineApi).add(state.form);
                ElMessage.success('新增成功');
            }
            state.visible = false;
            emit('refresh');
        }
    });
}

defineExpose({ open });
</script>
