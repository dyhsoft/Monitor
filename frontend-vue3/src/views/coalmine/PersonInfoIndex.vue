<template>
  <div>
    <n-card title="人员信息管理" :bordered="false" class="proCard">
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
          <n-input
            v-model:value="searchParams.personName"
            placeholder="姓名"
            clearable
            style="width: 120px"
          />
          <n-input
            v-model:value="searchParams.cardId"
            placeholder="卡号"
            clearable
            style="width: 150px"
          />
          <n-button type="primary" @click="loadData">搜索</n-button>
          <n-button @click="handleReset">重置</n-button>
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
    <n-modal v-model:show="modalVisible" preset="card" :title="modalTitle" style="width: 500px">
      <n-form ref="formRef" :model="formData" :rules="rules" label-placement="left">
        <n-form-item label="煤矿" path="mineId">
          <n-select v-model:value="formData.mineId" :options="mineOptions" />
        </n-form-item>
        <n-form-item label="姓名" path="personName">
          <n-input v-model:value="formData.personName" />
        </n-form-item>
        <n-form-item label="卡号" path="cardId">
          <n-input v-model:value="formData.cardId" />
        </n-form-item>
        <n-form-item label="部门">
          <n-input v-model:value="formData.department" />
        </n-form-item>
        <n-form-item label="工种">
          <n-input v-model:value="formData.workType" />
        </n-form-item>
        <n-form-item label="职位">
          <n-input v-model:value="formData.position" />
        </n-form-item>
        <n-form-item label="电话">
          <n-input v-model:value="formData.phone" />
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
  personName: '',
  cardId: ''
})

// 选项
const mineOptions = ref([])

// 表格列
const columns = [
  { title: 'ID', key: 'id', width: 60 },
  { title: '煤矿', key: 'mineName', width: 120 },
  { title: '姓名', key: 'personName', width: 100 },
  { title: '卡号', key: 'cardId', width: 140 },
  { title: '部门', key: 'department', width: 120 },
  { title: '工种', key: 'workType', width: 100 },
  { title: '职位', key: 'position', width: 100 },
  { title: '电话', key: 'phone', width: 120 },
  { title: '状态', key: 'status', width: 80, render: (row: any) => row.status === 1 ? '在岗' : '离岗' },
  { title: '创建时间', key: 'createTime', width: 160 }
]

// 弹窗
const modalVisible = ref(false)
const modalTitle = ref('新增人员')
const formRef = ref()
const formData = reactive({
  id: null,
  mineId: null,
  personName: '',
  cardId: '',
  department: '',
  workType: '',
  position: '',
  phone: '',
  status: 1
})

const rules = {
  mineId: { required: true, message: '请选择煤矿' },
  personName: { required: true, message: '请输入姓名' },
  cardId: { required: true, message: '请输入卡号' }
}

async function loadData() {
  loading.value = true
  try {
    // 模拟数据
    dataList.value = [
      { id: 1, mineName: '煤矿A', personName: '张三', cardId: '001', department: '采煤队', workType: '采煤工', position: '员工', phone: '13800138000', status: 1, createTime: '2024-01-01 10:00:00' },
      { id: 2, mineName: '煤矿A', personName: '李四', cardId: '002', department: '掘进队', workType: '掘进工', position: '班长', phone: '13800138001', status: 1, createTime: '2024-01-02 10:00:00' },
      { id: 3, mineName: '煤矿B', personName: '王五', cardId: '003', department: '通风队', workType: '通风工', position: '员工', phone: '13800138002', status: 0, createTime: '2024-01-03 10:00:00' }
    ]
    pagination.itemCount = dataList.value.length
  } finally {
    loading.value = false
  }
}

async function loadMineOptions() {
  try {
    const res = await getCoalMineList()
    mineOptions.value = (res.data || []).map((m: any) => ({ label: m.name, value: m.id }))
  } catch (e) {
    console.error('加载煤矿列表失败', e)
  }
}

function handleAdd() {
  formData.id = null
  formData.mineId = null
  formData.personName = ''
  formData.cardId = ''
  formData.department = ''
  formData.workType = ''
  formData.position = ''
  formData.phone = ''
  formData.status = 1
  modalTitle.value = '新增人员'
  modalVisible.value = true
}

function handleEdit(row: any) {
  Object.assign(formData, row)
  modalTitle.value = '编辑人员'
  modalVisible.value = true
}

async function handleDelete(row: any) {
  message.success('删除成功')
  loadData()
}

async function handleSubmit() {
  try {
    await formRef.value?.validate()
    message.success(formData.id ? '更新成功' : '新增成功')
    modalVisible.value = false
    loadData()
  } catch (e) {
    console.error('验证失败', e)
  }
}

function handleReset() {
  searchParams.mineId = null
  searchParams.personName = ''
  searchParams.cardId = ''
  loadData()
}

onMounted(() => {
  loadMineOptions()
  loadData()
})
</script>

<script lang="ts">
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
