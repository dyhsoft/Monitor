<template>
    <el-dialog v-model="state.visible" :title="state.title" width="600px" destroy-on-close>
        <el-form :model="state.form" ref="formRef" :rules="rules" label-width="100px">
            <el-form-item label="所属煤矿" prop="mineId">
                <el-select v-model="state.form.mineId" placeholder="请选择煤矿" filterable>
                    <el-option v-for="item in mineList" :key="item.id" :label="item.name" :value="item.id" />
                </el-select>
            </el-form-item>
            <el-form-item label="区域编码" prop="code">
                <el-input v-model="state.form.code" placeholder="请输入区域编码" />
            </el-form-item>
            <el-form-item label="区域名称" prop="name">
                <el-input v-model="state.form.name" placeholder="请输入区域名称" />
            </el-form-item>
            <el-form-item label="父级区域">
                <el-select v-model="state.form.parentId" placeholder="请选择父级区域" clearable filterable>
                    <el-option v-for="item in areaList" :key="item.id" :label="item.name" :value="item.id" />
                </el-select>
            </el-form-item>
            <el-form-item label="区域类型">
                <el-radio-group v-model="state.form.type">
                    <el-radio :value="1">大巷</el-radio>
                    <el-radio :value="2">工作面</el-radio>
                    <el-radio :value="3">硐室</el-radio>
                </el-radio-group>
            </el-form-item>
            <el-row :gutter="20">
                <el-col :span="8">
                    <el-form-item label="坐标X">
                        <el-input v-model="state.form.x" placeholder="大地坐标X" />
                    </el-form-item>
                </el-col>
                <el-col :span="8">
                    <el-form-item label="坐标Y">
                        <el-input v-model="state.form.y" placeholder="大地坐标Y" />
                    </el-form-item>
                </el-col>
                <el-col :span="8">
                    <el-form-item label="深度">
                        <el-input v-model="state.form.z" placeholder="深度" />
                    </el-form-item>
                </el-col>
            </el-row>
            <el-form-item label="容纳人数">
                <el-input-number v-model="state.form.capacity" :min="0" :max="10000" />
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
import { ref, reactive, computed, watch } from 'vue';
import { ElMessage } from 'element-plus';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineAreaApi } from '/@/api-services/api';

const props = defineProps<{
    mineList: any[]
}>();

const emit = defineEmits(['refresh']);
const formRef = ref();
const state = reactive({
    visible: false,
    title: '新增区域',
    form: {} as any,
    id: 0,
    areaList: [] as any[]
});

const enabled = computed({
    get: () => state.form.enabled || 1,
    set: (val) => state.form.enabled = val
});

const rules = {
    mineId: [{ required: true, message: '请选择煤矿', trigger: 'change' }],
    code: [{ required: true, message: '请输入区域编码', trigger: 'blur' }],
    name: [{ required: true, message: '请输入区域名称', trigger: 'blur' }]
};

watch(() => state.form.mineId, (val) => {
    if (val) {
        getAPI(CoalMineAreaApi).getList({ mineId: val, page: 1, pageSize: 1000 }).then((res) => {
            state.areaList = res.data.result || [];
        });
    } else {
        state.areaList = [];
    }
});

function open(row?: any) {
    state.visible = true;
    state.form = { type: 1, enabled: 1, capacity: 0 };
    state.id = 0;
    state.areaList = [];
    if (row) {
        state.title = '编辑区域';
        state.id = row.id;
        getAPI(CoalMineAreaApi).get(row.id).then((res) => {
            state.form = { ...res.data.result };
            if (state.form.mineId) {
                getAPI(CoalMineAreaApi).getList({ mineId: state.form.mineId, page: 1, pageSize: 1000 }).then((res2) => {
                    state.areaList = res2.data.result || [];
                });
            }
        });
    } else {
        state.title = '新增区域';
    }
}

function submit() {
    formRef.value?.validate(async (valid) => {
        if (valid) {
            if (state.id) {
                await getAPI(CoalMineAreaApi).update({ ...state.form, id: state.id });
                ElMessage.success('更新成功');
            } else {
                await getAPI(CoalMineAreaApi).add(state.form);
                ElMessage.success('新增成功');
            }
            state.visible = false;
            emit('refresh');
        }
    });
}

defineExpose({ open });
</script>
