<template>
  <div
    v-if="!tasks.length"
    class="empty"
  >
    <div class="empty-wrapper">
      <img src="/images/empty.png">
      <h4>Add a task to get started</h4>
    </div>
  </div>
</template>

<script>
export default {
  name: 'EmptyStateDisplay',
  props: {
    tasks: {
      type: Array,
      required: true
    }
  }
}

</script>

<style scoped>
.empty-wrapper {
  position: absolute;
  top: -10px;
  height: 500px;
  pointer-events: none;
}

.empty-wrapper img{
  max-width: 100%;
}

.empty-wrapper h4 {
  text-align: center;
  margin: 0px;
  position: relative;
  bottom: 70px;
  color: rgb(148, 148, 148);
}

.empty {
  position: relative;
}
</style>
