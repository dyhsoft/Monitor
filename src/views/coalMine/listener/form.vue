<template>
    <el-dialog v-model="state.visible" :title="state.title" width="600px" destroy-on-close>
        <el-form :model="state.form" ref="formRef" :rules="rules" label-width="100px">
            <el-form-item label="所属煤矿" prop="mineId">
                <el-select v-model="state.form.mineId" placeholder="请选择煤矿" filterable>
                    <el-option v-for="item in mineList" :key="item.id" :label="item.name" :value="item.id" />
                </el-select>
            </el-form-item>
            <el-form-item label="监听路径" prop="listenPath">
                <el-input v-model="state.form.listenPath" placeholder="请输入监听目录路径" />
            </el-form-item>
            <el-form-item label="数据类型" prop="dataType">
                <el-select v-model="state.form.dataType" placeholder="请选择数据类型">
                    <el-option label="CDSS(传感器数据)" value="CDSS" />
                    <el-option label="RWSS(人员定位)" value="RWSS" />
                    <el-option label="KYGL(矿压监测)" value="KYGL" />
                    <el-option label="SWJC(水文监测)" value="SWJC" />
                </el-select>
            </el-form-item>
            <el-form-item label="绑定系统" prop="bindSystem">
                <el-select v-model="state.form.bindSystem" placeholder="请选择绑定系统">
                    <el-option label="安全监测" value="安全监测" />
                    <el-option label="人员定位" value="人员定位" />
                    <el-option label="矿压监测" value="矿压监测" />
                    <el-option label="水文监测" value="水文监测" />
                </el-select>
            </el-form-item>
            <el-form-item label="文件匹配">
                <el-input v-model="state.form.filePattern" placeholder="默认 *.txt" />
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
import { ref, reactive, computed } from 'vue';
import { ElMessage } from 'element-plus';
import { getAPI } from '/@/utils/axios-utils';
import { ListenerConfigApi } from '/@/api-services/api';

const props = defineProps<{
    mineList: any[]
}>();

const emit = defineEmits(['refresh']);
const formRef = ref();
const state = reactive({
    visible: false,
    title: '新增监听配置',
    form: {} as any,
    id: 0
});

const enabled = computed({
    get: () => state.form.enabled || 1,
    set: (val) => state.form.enabled = val
});

const rules = {
    mineId: [{ required: true, message: '请选择煤矿', trigger: 'change' }],
    listenPath: [{ required: true, message: '请输入监听路径', trigger: 'blur' }],
    dataType: [{ required: true, message: '请选择数据类型', trigger: 'change' }],
    bindSystem: [{ required: true, message: '请选择绑定系统', trigger: 'change' }]
};

function open(row?: any) {
    state.visible = true;
    state.form = { enabled: 1, filePattern: '*.txt' };
    state.id = 0;
    if (row) {
        state.title = '编辑监听配置';
        state.id = row.id;
        getAPI(ListenerConfigApi).get(row.id).then((res) => {
            state.form = { ...res.data.result };
        });
    } else {
        state.title = '新增监听配置';
    }
}

function submit() {
    formRef.value?.validate(async (valid) => {
        if (valid) {
            if (state.id) {
                await getAPI(ListenerConfigApi).update({ ...state.form, id: state.id });
                ElMessage.success('更新成功');
            } else {
                await getAPI(ListenerConfigApi).add(state.form);
                ElMessage.success('新增成功');
            }
            state.visible = false;
            emit('refresh');
        }
    });
}

defineExpose({ open });
</script>
