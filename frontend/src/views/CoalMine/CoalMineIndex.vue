<template>
  <div>
    <BasicTable :tableConfig="tableConfig" :searchForm="searchForm" :data="dataList" :loading="loading">
      <template #toolbar>
        <a-button type="primary" @click="handleAdd">
          <template #icon><PlusOutlined /></template>
          新增煤矿
        </a-button>
      </template>
      
      <template #columns>
        <a-table-column title="煤矿编码" data-index="code" key="code" />
        <a-table-column title="煤矿名称" data-index="name" key="name" />
        <a-table-column title="所属集团" data-index="groupName" key="groupName" />
        <a-table-column title="矿井类型" data-index="mineType" key="mineType" />
        <a-table-column title="设计产能(万吨/年)" data-index="designCapacity" key="designCapacity" />
        <a-table-column title="联系人" data-index="contact" key="contact" />
        <a-table-column title="联系电话" data-index="phone" key="phone" />
        <a-table-column title="状态" data-index="status" key="status">
          <template #cell="{ record }">
            <a-tag :color="record.status === 1 ? 'green' : 'red'">
              {{ record.status === 1 ? '启用' : '停用' }}
            </a-tag>
          </template>
        </a-table-column>
        <a-table-column title="创建时间" data-index="createTime" key="createTime" />
        <a-table-column title="操作" key="action" width="180">
          <template #cell="{ record }">
            <a-space>
              <a-button type="link" size="small" @click="handleEdit(record)">编辑</a-button>
              <a-button type="link" size="small" @click="handleDetail(record)">详情</a-button>
              <a-button type="link" size="small" danger @click="handleDelete(record)">删除</a-button>
            </a-space>
          </template>
        </a-table-column>
      </template>
    </BasicTable>

    <!-- 新增/编辑弹窗 -->
    <a-modal
      v-model:open="modalVisible"
      :title="modalTitle"
      width="700px"
      @ok="handleSubmit"
      @cancel="modalVisible = false"
    >
      <a-form
        ref="formRef"
        :model="formData"
        :label-col="{ span: 6 }"
        :wrapper-col="{ span: 16 }"
      >
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="煤矿编码" name="code" :rules="[{ required: true, message: '请输入煤矿编码' }]">
              <a-input v-model:value="formData.code" placeholder="10位编码" :maxlength="10" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="煤矿名称" name="name" :rules="[{ required: true, message: '请输入煤矿名称' }]">
              <a-input v-model:value="formData.name" placeholder="煤矿名称" />
            </a-form-item>
          </a-col>
        </a-row>
        
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="所属集团" name="groupName">
              <a-input v-model:value="formData.groupName" placeholder="所属集团" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="矿井类型" name="mineType">
              <a-select v-model:value="formData.mineType" placeholder="请选择">
                <a-select-option value="生产">生产矿井</a-select-option>
                <a-select-option value="建设">建设矿井</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
        </a-row>
        
        <a-row :gutter="16">
          <a-col :span="8">
            <a-form-item label="省份" name="province">
              <a-input v-model:value="formData.province" placeholder="省" :maxlength="2" />
            </a-form-item>
          </a-col>
          <a-col :span="8">
            <a-form-item label="城市" name="city">
              <a-input v-model:value="formData.city" placeholder="市" :maxlength="2" />
            </a-form-item>
          </a-col>
          <a-col :span="8">
            <a-form-item label="县区" name="county">
              <a-input v-model:value="formData.county" placeholder="县" :maxlength="2" />
            </a-form-item>
          </a-col>
        </a-row>
        
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="设计产能" name="designCapacity">
              <a-input-number v-model:value="formData.designCapacity" :min="0" style="width: 100%" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="状态" name="status">
              <a-switch v-model:checked="formData.status" checked-value="1" unchecked-value="0" />
            </a-form-item>
          </a-col>
        </a-row>
        
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="联系人" name="contact">
              <a-input v-model:value="formData.contact" placeholder="联系人" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="联系电话" name="phone">
              <a-input v-model:value="formData.phone" placeholder="联系电话" />
            </a-form-item>
          </a-col>
        </a-row>
        
        <a-form-item label="详细地址" name="address">
          <a-textarea v-model:value="formData.address" placeholder="详细地址" :rows="2" />
        </a-form-item>
        
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="经度" name="longitude">
              <a-input-number v-model:value="formData.longitude" :min="-180" :max="180" :step="0.000001" style="width: 100%" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="纬度" name="latitude">
              <a-input-number v-model:value="formData.latitude" :min="-90" :max="90" :step="0.000001" style="width: 100%" />
            </a-form-item>
          </a-col>
        </a-row>
        
        <a-form-item label="备注" name="remark">
          <a-textarea v-model:value="formData.remark" placeholder="备注" :rows="2" />
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue';
import { message } from 'ant-design-vue';
import { BasicTable } from '@/components/Table';
import { PlusOutlined } from '@ant-design/icons-vue';

const loading = ref(false);
const dataList = ref([]);
const modalVisible = ref(false);
const modalTitle = ref('新增煤矿');
const formRef = ref(null);

const searchForm = reactive({
  code: '',
  name: '',
  status: null
});

const formData = reactive({
  id: null,
  code: '',
  name: '',
  groupName: '',
  province: '',
  city: '',
  county: '',
  mineType: '',
  designCapacity: null,
  contact: '',
  phone: '',
  address: '',
  longitude: null,
  latitude: null,
  status: '1',
  remark: ''
});

const tableConfig = reactive({
  api: '/api/CoalMine/CoalMine/page',
  method: 'POST',
  columns: [],
  pagination: true,
  showIndex: true
});

// 加载数据
const loadData = async () => {
  loading.value = true;
  try {
    // TODO: 调用API
    // const res = await http.post('/api/CoalMine/CoalMine/page', searchForm);
    // dataList.value = res.data.rows;
  } finally {
    loading.value = false;
  }
};

// 新增
const handleAdd = () => {
  modalTitle.value = '新增煤矿';
  Object.assign(formData, {
    id: null,
    code: '',
    name: '',
    groupName: '',
    province: '',
    city: '',
    county: '',
    mineType: '',
    designCapacity: null,
    contact: '',
    phone: '',
    address: '',
    longitude: null,
    latitude: null,
    status: '1',
    remark: ''
  });
  modalVisible.value = true;
};

// 编辑
const handleEdit = (record) => {
  modalTitle.value = '编辑煤矿';
  Object.assign(formData, record);
  modalVisible.value = true;
};

// 详情
const handleDetail = (record) => {
  modalTitle.value = '煤矿详情';
  Object.assign(formData, record);
  modalVisible.value = true;
};

// 删除
const handleDelete = async (record) => {
  // TODO: 调用删除API
  message.success('删除成功');
  loadData();
};

// 提交
const handleSubmit = async () => {
  // TODO: 调用API保存
  message.success('保存成功');
  modalVisible.value = false;
  loadData();
};

loadData();
</script>
