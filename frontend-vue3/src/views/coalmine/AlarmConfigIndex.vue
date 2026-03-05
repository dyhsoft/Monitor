<template>
  <div>
    <n-card title="报警配置" :bordered="false" class="proCard">
      <n-space vertical :size="16">
        <!-- 操作栏 -->
        <n-space>
          <n-button type="primary" @click="handleAdd">
            <template #icon>
              <n-icon><Add /></n-icon>
            </template>
            新增
          </n-button>
          <n-button @click="loadData">
            <template #icon>
              <n-icon><Refresh /></n-icon>
            </template>
            刷新
          </n-button>
        </n-space>

        <!-- 搜索栏 -->
        <n-space :size="12">
          <n-select
            v-model:value="searchParams.mineId"
            placeholder="选择煤矿"
            clearable
            :options="mineOptions"
            style="width: 200px"
          />
          <n-select
            v-model:value="searchParams.sensorTypeCode"
            placeholder="传感器类型"
            clearable
            :options="sensorTypeOptions"
            style="width: 180px"
          />
          <n-select
            v-model:value="searchParams.alarmEnabled"
            placeholder="状态"
            clearable
            :options="enabledOptions"
            style="width: 120px"
          />
          <n-button type="primary" @click="loadData">搜索</n-button>
        </n-space>

        <!-- 数据表格 -->
        <n-data-table
          :columns="columns"
          :data="dataList"
          :loading="loading"
          :pagination="pagination"
          :row-key="(row) => row.id"
        />
      </n-space>
    </n-card>

    <!-- 新增/编辑弹窗 -->
    <n-modal v-model:show="modalVisible" preset="card" :title="modalTitle" style="width: 600px">
      <n-form ref="formRef" :model="formData" :rules="rules" label-placement="left">
        <n-form-item label="煤矿" path="mineId">
          <n-select v-model:value="formData.mineId" :options="mineOptions" />
        </n-form-item>
        <n-form-item label="传感器类型" path="sensorTypeCode">
          <n-select v-model:value="formData.sensorTypeCode" :options="sensorTypeOptions" />
        </n-form-item>
        <n-form-item label="报警类型" path="alarmType">
          <n-select v-model:value="formData.alarmType" :options="alarmTypeOptions" />
        </n-form-item>
        <n-form-item label="报警级别" path="alarmLevel">
          <n-select v-model:value="formData.alarmLevel" :options="alarmLevelOptions" />
        </n-form-item>
        <n-form-item label="阈值" path="thresholdValue">
          <n-input-number v-model:value="formData.thresholdValue" :precision="4" style="width: 100%" />
        </n-form-item>
        <n-form-item label="阈值2" path="thresholdValue2">
          <n-input-number v-model:value="formData.thresholdValue2" :precision="4" style="width: 100%" />
        </n-form-item>
        <n-form-item label="报警延时(秒)" path="delaySeconds">
          <n-input-number v-model:value="formData.delaySeconds" :min="0" style="width: 100%" />
        </n-form-item>
        <n-form-item label="是否启用" path="alarmEnabled">
          <n-switch v-model:value="formData.alarmEnabled" />
        </n-form-item>
        <n-form-item label="备注" path="remark">
          <n-input v-model:value="formData.remark" type="textarea" />
        </n-form-item>
      </n-form>
      <template #footer>
        <n-space justify="end">
          <n-button @click="modalVisible = false">取消</n-button>
          <n-button type="primary" @click="handleSubmit">确定</n-button>
        </n-space>
      </template>
    </n-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { Add, Refresh } from '@vicons/tabler'
import {
  getAlarmConfigPage,
  addAlarmConfig,
  updateAlarmConfig,
  deleteAlarmConfig,
  getSensorTypes,
  getAlarmTypes,
  getAlarmLevels
} from '@/api/coalmine/alarm'
import { getCoalMineList } from '@/api/coalmine/coal'
import { useMessage } from 'naive-ui'

const message = useMessage()

const loading = ref(false)
const dataList = ref([])
const pagination = reactive({
  page: 1,
  pageSize: 10,
  itemCount: 0,
  showSizePicker: true,
  pageSizes: [10, 20, 50],
  showQuickJumper: true
})

const searchParams = reactive({
  mineId: null,
  sensorTypeCode: null,
  alarmEnabled: null
})

// 选项数据
const mineOptions = ref([])
const sensorTypeOptions = ref([])
const alarmTypeOptions = ref([])
const alarmLevelOptions = ref([])
const enabledOptions = [
  { label: '启用', value: 1 },
  { label: '禁用', value: 0 }
]

// 表格列
const columns = [
  { title: 'ID', key: 'id', width: 60 },
  { title: '煤矿', key: 'mineName', width: 120 },
  { title: '传感器类型', key: 'sensorTypeName', width: 120 },
  { title: '报警类型', key: 'alarmTypeName', width: 100 },
  { title: '报警级别', key: 'alarmLevel', width: 80, render: (row: any) => getAlarmLevelName(row.alarmLevel) },
  { title: '阈值', key: 'thresholdValue', width: 100 },
  { title: '阈值2', key: 'thresholdValue2', width: 100 },
  { title: '延时(秒)', key: 'delaySeconds', width: 80 },
  { title: '状态', key: 'alarmEnabled', width: 80, render: (row: any) => row.alarmEnabled ? '启用' : '禁用' },
  { title: '创建时间', key: 'createTime', width: 160, render: (row: any) => row.createTime?.substring(0, 19) },
  {
    title: '操作',
    key: 'actions',
    width: 120,
    render: (row: any) => {
      return [
        h(NButton, { size: 'small', onClick: () => handleEdit(row) }, () => '编辑'),
        h(NButton, { size: 'small', type: 'error', style: 'margin-left: 8px', onClick: () => handleDelete(row) }, () => '删除')
      ]
    }
  }
]

// 弹窗
const modalVisible = ref(false)
const modalTitle = ref('新增报警配置')
const formRef = ref()
const formData = reactive({
  id: null,
  mineId: null,
  sensorTypeCode: '',
  sensorTypeName: '',
  alarmType: 1,
  alarmLevel: 1,
  thresholdValue: null,
  thresholdValue2: null,
  delaySeconds: 0,
  alarmEnabled: 1,
  remark: ''
})

const rules = {
  mineId: { required: true, message: '请选择煤矿' },
  sensorTypeCode: { required: true, message: '请选择传感器类型' },
  alarmType: { required: true, message: '请选择报警类型' }
}

function getAlarmLevelName(level: number) {
  const map = { 1: '一般', 2: '重要', 3: '紧急', 4: '严重' }
  return map[level] || '未知'
}

async function loadData() {
  loading.value = true
  try {
    const res = await getAlarmConfigPage({
      current: pagination.page,
      size: pagination.pageSize,
      ...searchParams
    })
    dataList.value = res.data.rows || []
    pagination.itemCount = res.data.total || 0
  } finally {
    loading.value = false
  }
}

async function loadOptions() {
  try {
    // 加载煤矿列表
    const mineRes = await getCoalMineList()
    mineOptions.value = (mineRes.data || []).map((m: any) => ({ label: m.name, value: m.id }))

    // 加载传感器类型
    const typeRes = await getSensorTypes()
    sensorTypeOptions.value = (typeRes.data || []).map((t: any) => ({ label: t.name, value: t.code }))

    // 加载报警类型
    const atRes = await getAlarmTypes()
    alarmTypeOptions.value = (atRes.data || []).map((t: any) => ({ label: t.name, value: t.value }))

    // 加载报警级别
    const alRes = await getAlarmLevels()
    alarmLevelOptions.value = (alRes.data || []).map((t: any) => ({ label: t.name, value: t.value }))
  } catch (e) {
    console.error('加载选项失败', e)
  }
}

function handleAdd() {
  formData.id = null
  formData.mineId = null
  formData.sensorTypeCode = ''
  formData.sensorTypeName = ''
  formData.alarmType = 1
  formData.alarmLevel = 1
  formData.thresholdValue = null
  formData.thresholdValue2 = null
  formData.delaySeconds = 0
  formData.alarmEnabled = 1
  formData.remark = ''
  modalTitle.value = '新增报警配置'
  modalVisible.value = true
}

function handleEdit(row: any) {
  Object.assign(formData, row)
  modalTitle.value = '编辑报警配置'
  modalVisible.value = true
}

async function handleDelete(row: any) {
  try {
    await deleteAlarmConfig(row.id)
    message.success('删除成功')
    loadData()
  } catch (e: any) {
    message.error(e.message)
  }
}

async function handleSubmit() {
  try {
    await formRef.value?.validate()
    const data = { ...formData }
    
    // 获取传感器类型名称
    const type = sensorTypeOptions.value.find((t: any) => t.value === data.sensorTypeCode)
    if (type) data.sensorTypeName = type.label

    if (data.id) {
      await updateAlarmConfig(data)
      message.success('更新成功')
    } else {
      await addAlarmConfig(data)
      message.success('新增成功')
    }
    modalVisible.value = false
    loadData()
  } catch (e: any) {
    if (e.response) {
      message.error(e.response.data.message || '操作失败')
    }
  }
}

onMounted(() => {
  loadOptions()
  loadData()
})
</script>

<script lang="ts">
import { h } from 'vue'
import { NButton } from 'naive-ui'

export default {
  setup() {
    return {}
  }
}
</script>

<style scoped>
.proCard {
  background: #fff;
}
</style>
